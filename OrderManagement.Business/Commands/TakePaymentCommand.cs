using OrderManagement.Utility.IntegrationMessagePublisherSection;

namespace OrderManagement.Business.Commands
{
    public class TakePaymentCommand : IIntegrationCommand
    {
        public string CorrelationId { get; }
        public decimal TotalAmount { get; }

        public TakePaymentCommand(string correlationId, decimal totalAmount)
        {
            CorrelationId = correlationId;
            TotalAmount = totalAmount;
        }
    }
}