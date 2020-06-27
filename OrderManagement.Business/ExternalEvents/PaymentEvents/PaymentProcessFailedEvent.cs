using System;

namespace OrderManagement.Business.ExternalEvents.PaymentEvents
{
    public class PaymentProcessFailedEvent
    {
        public Guid OrderId
        {
            get => Guid.Parse(CorrelationId);
        }

        public string CorrelationId { get; private set; }

        public PaymentProcessFailedEvent(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}