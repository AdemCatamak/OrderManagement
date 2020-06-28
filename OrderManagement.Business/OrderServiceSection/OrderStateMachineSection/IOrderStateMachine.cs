using System.Threading.Tasks;
using OrderManagement.Business.OrderServiceSection.OrderStateMachineSection.Enums;

namespace OrderManagement.Business.OrderServiceSection.OrderStateMachineSection
{
    public interface IOrderStateMachine
    {
        OrderStates CurrentState { get; }

        Task TakePaymentAsync();
        Task ChangePaymentStatusAsync(PaymentStatuses paymentStatus);
        Task ChangeShipmentStatusAsync(ShipmentStatuses shipmentStatus);
        Task SetRefundCompletedAsync();
    }
}