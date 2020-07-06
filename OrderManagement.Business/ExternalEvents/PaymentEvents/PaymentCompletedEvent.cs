using System;

namespace OrderManagement.Business.ExternalEvents.PaymentEvents
{
    public class PaymentCompletedEvent
    {
        public string CorrelationId { get; }
        public DateTime PaymentDate { get; }

        public PaymentCompletedEvent(string correlationId, DateTime paymentDate)
        {
            CorrelationId = correlationId;
            PaymentDate = paymentDate;
        }
    }
}