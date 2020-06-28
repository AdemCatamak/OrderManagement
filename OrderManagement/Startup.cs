using System;
using System.Linq;
using System.Net.Http;
using GreenPipes;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OrderManagement.Api.Controllers;
using OrderManagement.Api.WebMiddleware;
using OrderManagement.Business.BillingServiceSection;
using OrderManagement.Business.OrderServiceSection;
using OrderManagement.Business.PaymentServiceSection;
using OrderManagement.Business.ShipmentServiceSection;
using OrderManagement.ConfigSection;
using OrderManagement.ConfigSection.ConfigModels;
using OrderManagement.Consumers;
using OrderManagement.Data;
using OrderManagement.Data.Models;
using OrderManagement.HostedServices;
using OrderManagement.MassTransitObservers;
using OrderManagement.Utility.IntegrationEventPublisherSection;

namespace OrderManagement
{
    public class Startup
    {
        private const string ALLOWED_ORIGIN_POLICY = "AllowedOriginPolicy";

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
                             {
                                 options.AddPolicy(ALLOWED_ORIGIN_POLICY,
                                                   builder =>
                                                   {
                                                       builder.AllowAnyHeader()
                                                              .AllowAnyMethod()
                                                              .AllowAnyOrigin();
                                                   });
                             });

            services.AddControllers()
                    .AddNewtonsoftJson(options =>
                                       {
                                           options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                                           options.SerializerSettings.DefaultValueHandling = DefaultValueHandling.Include;
                                           options.SerializerSettings.StringEscapeHandling = StringEscapeHandling.Default;
                                           options.SerializerSettings.TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Full;
                                           options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
                                           options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                                           options.SerializerSettings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
                                       })
                    .AddApplicationPart(typeof(HomeController).Assembly);

            services.AddHostedService<BusControlStarterHostedService>();

            #region Swagger

            services.AddSwaggerGen(c => { c.SwaggerDoc("v1", new OpenApiInfo()); });

            #endregion

            #region Db

            DbOption dbOption = AppConfigs.SelectedDbOption();

            switch (dbOption.DbType)
            {
                case DbTypes.SqlServer:
                    services.AddDbContext<DataContext>(builder => builder.UseSqlServer(dbOption.ConnectionStr));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            #endregion

            #region MassTransit

            MassTransitConfigModel massTransitConfigModel = AppConfigs.GetMassTransitConfigModel();
            MassTransitOption massTransitOption = massTransitConfigModel.SelectedMassTransitOption();

            services.AddSingleton(massTransitConfigModel);

            services.AddSingleton<IConsumeObserver, BasicConsumeObserver>();
            services.AddSingleton<ISendObserver, BasicSendObserver>();
            services.AddSingleton<IPublishObserver, BasicPublishObserver>();

            services.AddMassTransit(configurator =>
                                    {
                                        void ConfigureMassTransit(IBusFactoryConfigurator cfg)
                                        {
                                            cfg.UseInMemoryOutbox();

                                            cfg.UseConcurrencyLimit(massTransitConfigModel.ConcurrencyLimit);
                                            cfg.UseRetry(retryConfigurator => retryConfigurator.SetRetryPolicy(filter => filter.Incremental(massTransitConfigModel.RetryLimitCount, TimeSpan.FromSeconds(massTransitConfigModel.InitialIntervalSeconds), TimeSpan.FromSeconds(massTransitConfigModel.IntervalIncrementSeconds))));
                                        }

                                        configurator.AddSagaStateMachine<OrderStateMachine, OrderModel>()
                                                    .EntityFrameworkRepository(repositoryConfigurator => repositoryConfigurator.ExistingDbContext<DataContext>());

                                        configurator.AddBus(registrationContext =>
                                                            {
                                                                IBusControl busControl = massTransitOption.BrokerType switch
                                                                                         {
                                                                                             MassTransitBrokerTypes.RabbitMq
                                                                                             => Bus.Factory.CreateUsingRabbitMq(cfg =>
                                                                                                                                {
                                                                                                                                    cfg.Host(massTransitOption.HostName,
                                                                                                                                             massTransitOption.VirtualHost,
                                                                                                                                             hst =>
                                                                                                                                             {
                                                                                                                                                 hst.Username(massTransitOption.UserName);
                                                                                                                                                 hst.Password(massTransitOption.Password);
                                                                                                                                             });
                                                                                                                                    ConfigureMassTransit(cfg);
                                                                                                                                }),
                                                                                             _ => throw new ArgumentOutOfRangeException()
                                                                                         };

                                                                busControl.ConnectReceiveEndpoint($"{Program.STARTUP_PROJECT_NAME}.{nameof(OrderStateMachine)}",
                                                                                                  endpointConfigurator => { endpointConfigurator.StateMachineSaga<OrderModel>(registrationContext.Container); });


                                                                foreach (IConsumeObserver observer in registrationContext.Container.GetServices<IConsumeObserver>())
                                                                {
                                                                    busControl.ConnectConsumeObserver(observer);
                                                                }

                                                                foreach (ISendObserver observer in registrationContext.Container.GetServices<ISendObserver>())
                                                                {
                                                                    busControl.ConnectSendObserver(observer);
                                                                }

                                                                foreach (IPublishObserver observer in registrationContext.Container.GetServices<IPublishObserver>())
                                                                {
                                                                    busControl.ConnectPublishObserver(observer);
                                                                }

                                                                return busControl;
                                                            });
                                    });

            #endregion

            #region IntegrationEventPublisher

            services.AddScoped<IIntegrationEventPublisher, IntegrationEventPublisher>();

            #endregion

            #region HealthCheck

            IHealthChecksBuilder healthChecksBuilder = services.AddHealthChecks();

            healthChecksBuilder.AddUrlGroup(new Uri($"{AppConfigs.AppUrls().First()}/health-check"), HttpMethod.Get, name: "HealthCheck Endpoint");

            switch (dbOption.DbType)
            {
                case DbTypes.SqlServer:
                    healthChecksBuilder.AddSqlServer(dbOption.ConnectionStr, name: "Sql Server");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (massTransitOption.BrokerType)
            {
                case MassTransitBrokerTypes.RabbitMq:
                    string rabbitConnStr = $"amqp://{massTransitOption.UserName}:{massTransitOption.Password}@{massTransitOption.HostName}:5672{massTransitOption.VirtualHost}";
                    healthChecksBuilder.AddRabbitMQ(rabbitConnStr, sslOption: null, name: "RabbitMq", HealthStatus.Unhealthy, new[] {"rabbitmq"});
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            services
               .AddHealthChecksUI(setup =>
                                  {
                                      setup.MaximumHistoryEntriesPerEndpoint(50);
                                      setup.AddHealthCheckEndpoint("OrderManagement Project", $"{AppConfigs.AppUrls().First()}/healthz");
                                  })
               .AddInMemoryStorage();

            #endregion

            #region BusinessService

            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IBillingService, BillingService>();
            services.AddScoped<IShipmentService, ShipmentService>();

            #endregion
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMiddleware<GeneralExceptionHandlerMiddleware>();
            app.Use((async (httpContext, next) =>
                     {
                         if (httpContext.Request.Headers.TryGetValue("x-trace-id", out StringValues stringValues))
                         {
                             httpContext.TraceIdentifier = stringValues;
                         }

                         httpContext.TraceIdentifier ??= Guid.NewGuid().ToString();
                         await next();
                     }));
            app.UseMiddleware<TransactionMiddleware>();

            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", ""); });
            app.UseHealthChecksUI();
            app.UseHealthChecks("/healthz", new HealthCheckOptions
                                            {
                                                Predicate = _ => true,
                                                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                                            });


            app.UseRouting();
            app.UseCors(ALLOWED_ORIGIN_POLICY);
            app.UseEndpoints(builder => { builder.MapControllers(); });
        }
    }
}