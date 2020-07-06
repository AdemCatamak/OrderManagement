namespace OrderManagement.Business.ExternalEvents.PaymentEvents
{
    public class RefundStartedEvent
    {
        public string CorrelationId { get; }

        public RefundStartedEvent(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}