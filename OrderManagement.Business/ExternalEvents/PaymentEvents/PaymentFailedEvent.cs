namespace OrderManagement.Business.ExternalEvents.PaymentEvents
{
    public class PaymentFailedEvent
    {
        public long OrderId
        {
            get => long.Parse(CorrelationId);
        }

        public string CorrelationId { get; private set; }

        public PaymentFailedEvent(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}