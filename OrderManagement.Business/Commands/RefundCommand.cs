using OrderManagement.Utility.IntegrationMessagePublisherSection;

namespace OrderManagement.Business.Commands
{
    public class RefundCommand : IIntegrationCommand
    {
        public string CorrelationId { get; }

        public RefundCommand(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}