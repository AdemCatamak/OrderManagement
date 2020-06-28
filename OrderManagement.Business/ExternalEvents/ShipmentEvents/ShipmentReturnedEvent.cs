namespace OrderManagement.Business.ExternalEvents.ShipmentEvents
{
    public class ShipmentReturnedEvent
    {
        public long OrderId
        {
            get => long.Parse(CorrelationId);
        }

        public string CorrelationId { get; private set; }

        public ShipmentReturnedEvent(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}