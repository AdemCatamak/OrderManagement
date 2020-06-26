using System.Threading.Tasks;
using OrderManagement.Business.OrderServiceSection.Requests;
using OrderManagement.Business.OrderServiceSection.Responses;

namespace OrderManagement.Business.OrderServiceSection
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrderAsync(CreateOrderRequest createOrderRequest);
    }
}