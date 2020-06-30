using System.Threading.Tasks;
using GreenPipes;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderManagement.Data;
using OrderManagement.Utility.IntegrationMessagePublisherSection;
using IsolationLevel = System.Data.IsolationLevel;

namespace OrderManagement.Consumers.MassTransitMiddleware
{
    public class TransactionFilter<T> :
        IFilter<ConsumeContext<T>> where T : class
    {
        private readonly DataContext _dataContext;
        private readonly IIntegrationMessagePublisher _integrationMessagePublisher;

        public TransactionFilter(DataContext dataContext, IIntegrationMessagePublisher integrationMessagePublisher)
        {
            _dataContext = dataContext;
            _integrationMessagePublisher = integrationMessagePublisher;
        }


        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            IDbContextTransaction dbContextTransaction = await _dataContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            await next.Send(context);
            await dbContextTransaction.CommitAsync();
            await _integrationMessagePublisher.Publish();
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}