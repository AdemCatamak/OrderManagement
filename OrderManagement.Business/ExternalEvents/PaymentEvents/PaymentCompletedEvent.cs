using System;

namespace OrderManagement.Business.ExternalEvents.PaymentEvents
{
    public class PaymentCompletedEvent
    {
        public long OrderId
        {
            get => long.Parse(CorrelationId);
        }

        public string CorrelationId { get; private set; }
        public DateTime PaymentDate { get; private set; }

        public PaymentCompletedEvent(string correlationId, DateTime paymentDate)
        {
            CorrelationId = correlationId;
            PaymentDate = paymentDate;
        }
    }
}