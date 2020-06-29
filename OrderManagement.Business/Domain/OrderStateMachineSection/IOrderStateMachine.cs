using OrderManagement.Business.Domain.OrderServiceSection.Responses;
using OrderManagement.Business.Domain.OrderStateMachineSection.Enums;

namespace OrderManagement.Business.Domain.OrderStateMachineSection
{
    public interface IOrderStateMachine
    {
        OrderStates CurrentState { get; }
        OrderResponse OrderResponse { get; }

        void SubmitOrder();
        void SetAsPaymentStarted();
        void ChangePaymentStatus(PaymentStatuses paymentStatus);

        void SetAsOrderShipped();
        void ChangeShipmentStatus(ShipmentStatuses shipmentStatus);

        void SetAsRefundStarted();
        void SetAsRefundCompleted();
    }
}