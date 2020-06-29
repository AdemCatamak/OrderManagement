using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using OrderManagement.Business.ExternalEvents.PaymentEvents;

namespace OrderManagement.Business.Clients
{
    public interface IPaymentServiceClient
    {
        Task TakePaymentAsync(string correlationId, decimal totalAmount);
        Task RefundAsync(string correlationId);
    }

    public class PaymentServiceClient : IPaymentServiceClient
    {
        private readonly ILogger<PaymentServiceClient> _logger;
        private readonly IBusControl _busControl;

        public PaymentServiceClient(ILogger<PaymentServiceClient> logger, IBusControl busControl)
        {
            _logger = logger;
            _busControl = busControl;
        }

        public async Task TakePaymentAsync(string correlationId, decimal totalAmount)
        {
            _logger.LogInformation($"{correlationId} - Payment process is crated. Total amount: {totalAmount}");
            await _busControl.Publish(new PaymentCreatedEvent(correlationId));

            await Task.Delay(TimeSpan.FromSeconds(5));

            if (totalAmount > 1000)
            {
                _logger.LogInformation($"{correlationId} - Payment is failed. Total amount: {totalAmount}");
                await _busControl.Publish(new PaymentFailedEvent(correlationId));
            }
            else
            {
                _logger.LogInformation($"{correlationId} - Payment is completed. Total amount: {totalAmount}");
                await _busControl.Publish(new PaymentCompletedEvent(correlationId, DateTime.UtcNow));
            }
        }

        public async Task RefundAsync(string correlationId)
        {
            _logger.LogInformation($"{correlationId} - Refund is started");
            await _busControl.Publish(new RefundStartedEvent(correlationId));
            await Task.Delay(TimeSpan.FromSeconds(5));

            _logger.LogInformation($"{correlationId} - Refund is completed");
            await _busControl.Publish(new RefundCompletedEvent(correlationId, DateTime.UtcNow));
        }
    }
}