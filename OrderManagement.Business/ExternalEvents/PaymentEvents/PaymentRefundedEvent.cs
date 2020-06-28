using System;

namespace OrderManagement.Business.ExternalEvents.PaymentEvents
{
    public class PaymentRefundedEvent
    {
        public long OrderId
        {
            get => long.Parse(CorrelationId);
        }

        public string CorrelationId { get; private set; }
        public DateTime RefundDate { get; private set; }

        public PaymentRefundedEvent(string correlationId, DateTime refundDate)
        {
            CorrelationId = correlationId;
            RefundDate = refundDate;
        }
    }
}