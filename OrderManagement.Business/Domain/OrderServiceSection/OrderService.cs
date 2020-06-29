using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Business.Domain.Exceptions;
using OrderManagement.Business.Domain.OrderServiceSection.Mappings;
using OrderManagement.Business.Domain.OrderServiceSection.Requests;
using OrderManagement.Business.Domain.OrderServiceSection.Responses;
using OrderManagement.Data;
using OrderManagement.Data.Models;
using OrderManagement.Exceptions;

namespace OrderManagement.Business.Domain.OrderServiceSection
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _dataContext;

        public OrderService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<OrderCollectionResponse> QueryOrderAsync(QueryOrderRequest queryOrderRequest)
        {
            if (queryOrderRequest == null) throw new RequestNullException();

            IQueryable<OrderModel> orderModels = _dataContext.OrderModels.AsQueryable();

            if (queryOrderRequest.OrderId.HasValue)
                orderModels = orderModels.Where(m => m.Id == queryOrderRequest.OrderId);

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