using System.Threading.Tasks;
using OrderManagement.Business.Domain.OrderServiceSection.Requests;
using OrderManagement.Business.Domain.OrderServiceSection.Responses;

namespace OrderManagement.Business.Domain.OrderServiceSection
{
    public interface IOrderService
    {
        Task<OrderCollectionResponse> QueryOrderAsync(QueryOrderRequest queryOrderRequest);
    }
}