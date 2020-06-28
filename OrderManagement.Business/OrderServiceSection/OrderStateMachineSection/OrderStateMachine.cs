using System;
using System.Threading.Tasks;
using OrderManagement.Business.OrderServiceSection.Exceptions;
using OrderManagement.Business.OrderServiceSection.OrderStateMachineSection.Enums;
using OrderManagement.Business.PaymentServiceSection;
using OrderManagement.Business.ShipmentServiceSection;
using OrderManagement.Data.Models;
using Stateless;

namespace OrderManagement.Business.OrderServiceSection.OrderStateMachineSection
{
    public class OrderStateMachine : IOrderStateMachine
    {
        private readonly OrderModel _orderModel;
        private readonly StateMachine<OrderStates, OrderActions> _orderStateMachine;

        #region Triggers

        private readonly StateMachine<OrderStates, OrderActions>.TriggerWithParameters<PaymentStatuses> _changePaymentStatusTrigger;
        private readonly StateMachine<OrderStates, OrderActions>.TriggerWithParameters<ShipmentStatuses> _changeShipmentStatusTrigger;

        #endregion

        private readonly IPaymentService _paymentService;
        private readonly IShipmentService _shipmentService;

        public OrderStateMachine(OrderModel orderModel, IPaymentService paymentService, IShipmentService shipmentService)
        {
            _orderModel = orderModel;
            _paymentService = paymentService;
            _shipmentService = shipmentService;

            _orderStateMachine = new StateMachine<OrderStates, OrderActions>((OrderStates) _orderModel.OrderState);

            _changePaymentStatusTrigger = _orderStateMachine.SetTriggerParameters<PaymentStatuses>(OrderActions.ChangePaymentStatus);
            _changeShipmentStatusTrigger = _orderStateMachine.SetTriggerParameters<ShipmentStatuses>(OrderActions.ChangeShipmentStatus);


            _orderStateMachine.Configure(OrderStates.Initial)
                              .Permit(OrderActions.TakePayment, OrderStates.PaymentProcessStarted);

            _orderStateMachine.Configure(OrderStates.PaymentProcessStarted)
                              .OnEntryAsync(async () => await OnPaymentProcessStartedAsync())
                              .PermitDynamic(_changePaymentStatusTrigger, OnSetPaymentStatus);

            _orderStateMachine.Configure(OrderStates.PaymentCompleted)
                              .OnEntryAsync(async () => await OnPaymentCompletedAsync())
                              .Permit(OrderActions.PrepareShipment, OrderStates.OrderShipped);

            _orderStateMachine.Configure(OrderStates.OrderShipped)
                              .OnEntryAsync(async () => await OnOrderShipped())
                              .PermitDynamic(_changeShipmentStatusTrigger, OnSetShipmentStatus);

            _orderStateMachine.Configure(OrderStates.ShipmentReturned)
                              .OnEntryAsync(async () => await OnShipmentReturned())
                              .Permit(OrderActions.Refund, OrderStates.RefundStarted);

            _orderStateMachine.Configure(OrderStates.RefundStarted)
                              .OnEntryAsync(async () => await OnRefundStarted())
                              .Permit(OrderActions.SetRefundCompleted, OrderStates.RefundCompleted);

            _orderStateMachine.OnTransitioned(transition => _orderModel.SetState((int) transition.Destination));
            _orderStateMachine.OnUnhandledTriggerAsync((states, actions) => throw new InvalidStatusTransitionException(states.ToString(), actions.ToString()));
        }

        private async Task OnPaymentCompletedAsync()
        {
            await _orderStateMachine.FireAsync(OrderActions.PrepareShipment);
        }

        private async Task OnRefundStarted()
        {
            await _paymentService.RefundAsync(_orderModel.Id.ToString());
        }

        private async Task OnShipmentReturned()
        {
            await _orderStateMachine.FireAsync(OrderActions.Refund);
        }

        private async Task OnOrderShipped()
        {
            await _shipmentService.CreateShipmentAsync(_orderModel.Id.ToString(), _orderModel.BuyerName, _orderModel.BuyerAddress);
        }

        private static OrderStates OnSetShipmentStatus(ShipmentStatuses status)
        {
            return status switch
                   {
                       ShipmentStatuses.Delivered => OrderStates.ShipmentDelivered,
                       ShipmentStatuses.Returned => OrderStates.ShipmentReturned,
                       _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
                   };
        }

        private static OrderStates OnSetPaymentStatus(PaymentStatuses status)
        {
            return status switch
                   {
                       PaymentStatuses.Completed => OrderStates.PaymentCompleted,
                       PaymentStatuses.Failed => OrderStates.PaymentFailed,
                       _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
                   };
        }

        private async Task OnPaymentProcessStartedAsync()
        {
            await _paymentService.TakePaymentAsync(_orderModel.Id.ToString(), _orderModel.TotalAmount);
        }


        public OrderStates CurrentState => _orderStateMachine.State;

        public async Task TakePaymentAsync()
        {
            await _orderStateMachine.FireAsync(OrderActions.TakePayment);
        }

        public async Task ChangePaymentStatusAsync(PaymentStatuses paymentStatus)
        {
            await _orderStateMachine.FireAsync(_changePaymentStatusTrigger, paymentStatus);
        }

        public async Task ChangeShipmentStatusAsync(ShipmentStatuses shipmentStatus)
        {
            await _orderStateMachine.FireAsync(_changeShipmentStatusTrigger, shipmentStatus);
        }

        public async Task SetRefundCompletedAsync()
        {
            await _orderStateMachine.FireAsync(OrderActions.SetRefundCompleted);
        }
    }
}