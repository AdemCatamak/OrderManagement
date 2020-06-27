using System;

namespace OrderManagement.Business.ExternalEvents.BillingEvents
{
    public class BillingProcessCancelledEvent
    {
        public Guid OrderId
        {
            get => Guid.Parse(CorrelationId);
        }

        public string CorrelationId { get; private set; }

        public BillingProcessCancelledEvent(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}