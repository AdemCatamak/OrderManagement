using System;

namespace OrderManagement.Business.ExternalEvents.BillingEvents
{
    public class BillingProcessCompletedEvent
    {
        public Guid OrderId
        {
            get => Guid.Parse(CorrelationId);
        }

        public string CorrelationId { get; private set; }

        public BillingProcessCompletedEvent(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}