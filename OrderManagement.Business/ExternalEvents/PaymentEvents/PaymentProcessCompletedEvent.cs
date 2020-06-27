using System;

namespace OrderManagement.Business.ExternalEvents.PaymentEvents
{
    public class PaymentProcessCompletedEvent
    {
        public Guid OrderId
        {
            get => Guid.Parse(CorrelationId);
        }

        public string CorrelationId { get; private set; }
        public DateTime PaymentDate { get; private set; }

        public PaymentProcessCompletedEvent(string correlationId, DateTime paymentDate)
        {
            CorrelationId = correlationId;
            PaymentDate = paymentDate;
        }
    }
}