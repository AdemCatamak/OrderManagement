using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace OrderManagement.Business.BillingServiceSection
{
    public class BillingService : IBillingService
    {
        private readonly ILogger<BillingService> _logger;

        public BillingService(ILogger<BillingService> logger)
        {
            _logger = logger;
        }

        public Task CreateBill(string correlationId, decimal totalAmount, string buyerName)
        {
            _logger.LogInformation($"{correlationId} - ${totalAmount} bill is generated for {buyerName} ");
            return Task.CompletedTask;
        }

        public Task CancelBillAsync(string correlationId)
        {
            _logger.LogInformation($"{correlationId} - bill is cancelled");
            return Task.CompletedTask;
        }
    }
}