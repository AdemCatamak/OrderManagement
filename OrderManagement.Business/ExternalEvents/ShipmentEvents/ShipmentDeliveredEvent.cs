using System;

namespace OrderManagement.Business.ExternalEvents.ShipmentEvents
{
    public class ShipmentDeliveredEvent
    {
        public string CorrelationId { get; }

        public DateTime DeliveredOn { get; }

        public ShipmentDeliveredEvent(string correlationId, DateTime deliveredOn)
        {
            CorrelationId = correlationId;
            DeliveredOn = deliveredOn;
        }
    }
}