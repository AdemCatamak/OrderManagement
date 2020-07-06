using System.Threading.Tasks;

namespace OrderManagement.Business.Domain.OrderStateMachineSection
{
    public interface IOrderStateMachineFactory
    {
        Task<IOrderStateMachine> CreateOrderStateMachineAsync(string buyerName, string buyerAddress, decimal totalAmount);
        Task<IOrderStateMachine> BuildOrderStateMachineAsync(long orderId);
    }
}