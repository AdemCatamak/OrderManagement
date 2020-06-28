using System;

namespace OrderManagement.Business.ExternalEvents.ShipmentEvents
{
    public class ShipmentDeliveredEvent
    {
        public long OrderId
        {
            get => long.Parse(CorrelationId);
        }

        public string CorrelationId { get; private set; }

        public DateTime CompletedOn { get; private set; }

        public ShipmentDeliveredEvent(string correlationId, DateTime completedOn)
        {
            CorrelationId = correlationId;
            CompletedOn = completedOn;
        }
    }
}