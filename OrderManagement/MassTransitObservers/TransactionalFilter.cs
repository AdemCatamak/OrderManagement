using System;
using System.Data;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Data;
using OrderManagement.Utility.IntegrationEventPublisherSection;

namespace OrderManagement.MassTransitObservers
{
    public class TransactionalFilter<T> : IFilter<T> where T : class, ConsumeContext
    {
        private readonly IServiceProvider _serviceProvider;

        public TransactionalFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Send(T context, IPipe<T> next)
        {
            using (IServiceScope serviceScope = _serviceProvider.CreateScope())
            {
                var dataContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
                var integrationEventPublisher = serviceScope.ServiceProvider.GetRequiredService<IIntegrationEventPublisher>();

                IDbContextTransaction dbContextTransaction = await dataContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
                await next.Send(context);
                await dbContextTransaction.CommitAsync();
                await integrationEventPublisher.Publish();
            }
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}