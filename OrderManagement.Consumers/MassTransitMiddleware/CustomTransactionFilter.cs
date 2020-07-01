using System;
using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using OrderManagement.Data;
using OrderManagement.Utility.IntegrationMessagePublisherSection;
using IsolationLevel = System.Data.IsolationLevel;

namespace OrderManagement.Consumers.MassTransitMiddleware
{
    public class CustomTransactionFilter<TConsumer> :
        IFilter<ConsumerConsumeContext<TConsumer>> where TConsumer : class
    {
        public async Task Send(ConsumerConsumeContext<TConsumer> context, IPipe<ConsumerConsumeContext<TConsumer>> next)
        {
            var serviceProvider = context.GetPayload<IServiceProvider>();
            var dataContext = serviceProvider.GetRequiredService<DataContext>();
            var integrationMessagePublisher = serviceProvider.GetRequiredService<IIntegrationMessagePublisher>();

            IDbContextTransaction dbContextTransaction = await dataContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            await next.Send(context);
            await dbContextTransaction.CommitAsync();
            await integrationMessagePublisher.Publish();
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("TransactionFilter");
        }
    }
}