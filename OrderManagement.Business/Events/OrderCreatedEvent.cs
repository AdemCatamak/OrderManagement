using OrderManagement.Business.OrderServiceSection.Responses;
using OrderManagement.Utility.IntegrationEventPublisherSection;

namespace OrderManagement.Business.Events
{
    public class OrderCreatedEvent : IIntegrationEvent
    {
        public OrderResponse OrderResponse { get; }

        public OrderCreatedEvent(OrderResponse orderResponse)
        {
            OrderResponse = orderResponse;
        }
    }
}