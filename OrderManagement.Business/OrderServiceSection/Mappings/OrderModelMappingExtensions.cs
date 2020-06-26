using OrderManagement.Business.OrderServiceSection.Responses;
using OrderManagement.Data.Models;

namespace OrderManagement.Business.OrderServiceSection.Mappings
{
    public static class OrderModelMappingExtensions
    {
        public static OrderResponse ToOrderResponse(this OrderModel orderModel)
        {
            if (orderModel == null) return null;

            var orderResponse = new OrderResponse(orderModel.Id,
                                                  orderModel.CreatedOn,
                                                  orderModel.UpdatedOn,
                                                  orderModel.BuyerName,
                                                  orderModel.BuyerAddress,
                                                  orderModel.TotalAmount,
                                                  orderModel.OrderState);

            return orderResponse;
        }
    }
}