using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using OrderManagement.Data;
using OrderManagement.Utility.IntegrationEventPublisherSection;

namespace OrderManagement.Api.WebMiddleware
{
    public class TransactionMiddleware
    {
        private readonly RequestDelegate _next;

        public TransactionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, DataContext dataContext, IIntegrationEventPublisher integrationEventPublisher)
        {
            IDbContextTransaction dbContextTransaction = await dataContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted);
            await _next(context);
            await dbContextTransaction.CommitAsync();
            await integrationEventPublisher.Publish();
        }
    }
}