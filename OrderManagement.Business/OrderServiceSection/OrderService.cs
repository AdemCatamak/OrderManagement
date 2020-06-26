using System.Threading.Tasks;
using OrderManagement.Business.Events;
using OrderManagement.Business.OrderServiceSection.Mappings;
using OrderManagement.Business.OrderServiceSection.Requests;
using OrderManagement.Business.OrderServiceSection.Responses;
using OrderManagement.Data;
using OrderManagement.Data.Models;
using OrderManagement.Exceptions;
using OrderManagement.Utility.IntegrationEventPublisherSection;

namespace OrderManagement.Business.OrderServiceSection
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _dataContext;
        private readonly IIntegrationEventPublisher _integrationEventPublisher;

        public OrderService(DataContext dataContext, IIntegrationEventPublisher integrationEventPublisher)
        {
            _dataContext = dataContext;
            _integrationEventPublisher = integrationEventPublisher;
        }

        public async Task<OrderResponse> CreateOrderAsync(CreateOrderRequest createOrderRequest)
        {
            if (createOrderRequest == null) throw new RequestNullException();

            var orderModel = new OrderModel(createOrderRequest.BuyerName, createOrderRequest.BuyerAddress, createOrderRequest.TotalAmount);

            await _dataContext.OrderModels.AddAsync(orderModel);
            await _dataContext.SaveChangesAsync();

            var orderResponse = orderModel.ToOrderResponse();
            var orderCreatedEvent = new OrderCreatedEvent(orderResponse);
            _integrationEventPublisher.AddEvent(orderCreatedEvent);

            return orderResponse;
        }
    }
}