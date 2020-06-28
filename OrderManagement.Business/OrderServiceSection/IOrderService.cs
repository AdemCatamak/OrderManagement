using System.Threading.Tasks;
using OrderManagement.Business.OrderServiceSection.OrderStateMachineSection.Enums;
using OrderManagement.Business.OrderServiceSection.Requests;
using OrderManagement.Business.OrderServiceSection.Responses;

namespace OrderManagement.Business.OrderServiceSection
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest createOrderRequest);
        Task<OrderResponse> GetOrderAsync(long orderId);
        Task<OrderCollectionResponse> QueryOrderAsync(QueryOrderRequest queryOrderRequest);
        Task TakePaymentAsync(long orderId);
        Task ChangePaymentProcessStatusAsync(long orderId, PaymentStatuses paymentStatus);
        Task ChangeShipmentStatusAsync(long orderId, ShipmentStatuses shipmentStatus);
        Task SetRefundCompletedAsync(long orderId);
    }
}