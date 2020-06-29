namespace OrderManagement.Business.ExternalEvents.PaymentEvents
{
    public class PaymentCreatedEvent
    {
        public string CorrelationId { get; }

        public PaymentCreatedEvent(string correlationId)
        {
            CorrelationId = correlationId;
        }
    }
}