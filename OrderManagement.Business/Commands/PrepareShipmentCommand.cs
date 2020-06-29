using OrderManagement.Utility.IntegrationMessagePublisherSection;

namespace OrderManagement.Business.Commands
{
    public class PrepareShipmentCommand : IIntegrationCommand
    {
        public string CorrelationId { get; }
        public string ReceiverName { get; }
        public string ReceiverAddress { get; }

        public PrepareShipmentCommand(string correlationId, string receiverName, string receiverAddress)
        {
            CorrelationId = correlationId;
            ReceiverName = receiverName;
            ReceiverAddress = receiverAddress;
        }
    }
}