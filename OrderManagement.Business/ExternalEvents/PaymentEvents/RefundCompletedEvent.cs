using System;

namespace OrderManagement.Business.ExternalEvents.PaymentEvents
{
    public class RefundCompletedEvent
    {
        public string CorrelationId { get; }
        public DateTime CompletedOn { get; }

        public RefundCompletedEvent(string correlationId, DateTime completedOn)
        {
            CorrelationId = correlationId;
            CompletedOn = completedOn;
        }
    }
}