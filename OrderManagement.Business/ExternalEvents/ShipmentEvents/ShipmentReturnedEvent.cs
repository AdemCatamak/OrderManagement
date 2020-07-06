namespace OrderManagement.Business.ExternalEvents.ShipmentEvents
{
    public class ShipmentReturnedEvent
    {
        public string CorrelationId { get; }

        public ShipmentReturnedEvent(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}