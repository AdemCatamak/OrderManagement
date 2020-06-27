using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using OrderManagement.Business.ExternalEvents.PaymentEvents;

namespace OrderManagement.Business.PaymentServiceSection
{
    public class PaymentService : IPaymentService
    {
        private readonly ILogger<PaymentService> _logger;
        private readonly IBusControl _busControl;

        public PaymentService(ILogger<PaymentService> logger, IBusControl busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }

        public async Task TakePaymentAsync(string correlationId, decimal totalAmount)
        {
            if (totalAmount > 1000)
            {
                _logger.LogInformation($"{correlationId} - {totalAmount} could not taken from credit card");
                await _busControl.Publish(new PaymentProcessFailedEvent(correlationId));
            }
            else
            {
                _logger.LogInformation($"{correlationId} - {totalAmount} is taken from credit card");
                await _busControl.Publish(new PaymentProcessCompletedEvent(correlationId, DateTime.UtcNow));
            }
        }

        public async Task RefundAsync(string correlationId)
        {
            _logger.LogInformation($"{correlationId} - Refund process triggered");
            await _busControl.Publish(new PaymentRefundedEvent(correlationId, DateTime.UtcNow));
        }
    }
}