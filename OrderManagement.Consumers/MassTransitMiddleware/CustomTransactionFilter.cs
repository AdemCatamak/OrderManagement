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
            if (context.TryGetPayload(out IServiceProvider serviceProvider))
            {
                var dataContext = serviceProvider.GetRequiredService<DataContext>();
                var integrationMessagePublisher = serviceProvider.GetRequiredService<IIntegrationMessagePublisher>();

                IDbContextTransaction dbContextTransaction = await dataContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
                await next.Send(context);
                await dbContextTransaction.CommitAsync();
                await integrationMessagePublisher.Publish();
            }
            else
            {
                await next.Send(context);
            }
        }

        public void Probe(ProbeContext context)
        {
            context.CreateScope("TransactionFilter");
        }
    }
}