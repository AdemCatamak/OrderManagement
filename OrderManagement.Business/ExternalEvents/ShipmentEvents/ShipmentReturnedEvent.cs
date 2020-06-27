using System;

namespace OrderManagement.Business.ExternalEvents.ShipmentEvents
{
    public class ShipmentReturnedEvent
    {
        public Guid OrderId
        {
            get => Guid.Parse(CorrelationId);
        }

        public string CorrelationId { get; private set; }

        public ShipmentReturnedEvent(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}