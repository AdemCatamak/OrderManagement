namespace OrderManagement.Business.ExternalEvents.PaymentEvents
{
    public class PaymentFailedEvent
    {
        public string CorrelationId { get; }

        public PaymentFailedEvent(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}