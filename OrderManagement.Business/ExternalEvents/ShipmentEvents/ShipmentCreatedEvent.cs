namespace OrderManagement.Business.ExternalEvents.ShipmentEvents
{
    public class ShipmentCreatedEvent
    {
        public string CorrelationId { get; }

        public ShipmentCreatedEvent(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}