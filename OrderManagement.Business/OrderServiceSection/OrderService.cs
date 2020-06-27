using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Business.Events;
using OrderManagement.Business.OrderServiceSection.Exceptions;
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

        public async Task<OrderResponse> GetOrderAsync(Guid orderId)
        {
            OrderModel orderModel = await _dataContext.OrderModels.FirstOrDefaultAsync(m => m.OrderId == orderId);

            if (orderModel == null) throw new OrderNotFoundException(orderId);

            var orderResponse = orderModel.ToOrderResponse();

            return orderResponse;
        }

        public async Task<OrderCollectionResponse> QueryOrderAsync(QueryOrderRequest queryOrderRequest)
        {
            if (queryOrderRequest == null) throw new RequestNullException();

            IQueryable<OrderModel> orderModels = _dataContext.OrderModels.AsQueryable();

            if (queryOrderRequest.OrderId.HasValue)
                orderModels = orderModels.Where(m => m.OrderId == queryOrderRequest.OrderId);

            int totalCount = await orderModels.CountAsync();
            List<OrderModel> orderModelList = orderModels.Skip(queryOrderRequest.Offset)
                                                         .Take(queryOrderRequest.Take)
                                                         .ToList();

            if (!orderModelList.Any()) throw new OrderNotFoundException();

            List<OrderResponse> orderResponseList = orderModelList.Select(x => x.ToOrderResponse())
                                                                  .ToList();

            return new OrderCollectionResponse(totalCount, orderResponseList);
        }
    }
}