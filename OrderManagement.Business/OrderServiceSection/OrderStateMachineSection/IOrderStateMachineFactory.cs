using OrderManagement.Data.Models;

namespace OrderManagement.Business.OrderServiceSection.OrderStateMachineSection
{
    public interface IOrderStateMachineFactory
    {
        IOrderStateMachine BuildOrderStateMachine(OrderModel orderModel);
    }
}