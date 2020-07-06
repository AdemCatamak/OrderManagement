using System;
using OrderManagement.Business.Commands;
using OrderManagement.Business.Domain.Exceptions;
using OrderManagement.Business.Domain.OrderServiceSection.Mappings;
using OrderManagement.Business.Domain.OrderServiceSection.Responses;
using OrderManagement.Business.Domain.OrderStateMachineSection.Enums;
using OrderManagement.Data;
using OrderManagement.Data.Models;
using OrderManagement.Utility.IntegrationMessagePublisherSection;
using Stateless;

namespace OrderManagement.Business.Domain.OrderStateMachineSection
{
    public class OrderStateMachine : IOrderStateMachine
    {
        private readonly IIntegrationMessagePublisher _integrationMessagePublisher;

        private readonly OrderModel _orderModel;
        private readonly StateMachine<OrderStates, OrderActions> _orderStateMachine;

        #region Triggers

        private readonly StateMachine<OrderStates, OrderActions>.TriggerWithParameters<PaymentStatuses> _changePaymentStatusTrigger;
        private readonly StateMachine<OrderStates, OrderActions>.TriggerWithParameters<ShipmentStatuses> _changeShipmentStatusTrigger;

        #endregion

        public OrderStateMachine(OrderModel orderModel,
                                 IIntegrationMessagePublisher integrationMessagePublisher,
                                 DataContext dataContext)
        {
            _integrationMessagePublisher = integrationMessagePublisher;

            _orderModel = orderModel;
            _orderStateMachine = new StateMachine<OrderStates, OrderActions>((OrderStates) _orderModel.OrderState);

            _changePaymentStatusTrigger = _orderStateMachine.SetTriggerParameters<PaymentStatuses>(OrderActions.ChangePaymentStatus);
            _changeShipmentStatusTrigger = _orderStateMachine.SetTriggerParameters<ShipmentStatuses>(OrderActions.ChangeShipmentStatus);

            _orderStateMachine.Configure(OrderStates.Initial)
                              .Permit(OrderActions.Submit, OrderStates.Submitted);

            _orderStateMachine.Configure(OrderStates.Submitted)
                              .OnEntry(OnSubmitted)
                              .Permit(OrderActions.SetAsPaymentStarted, OrderStates.PaymentProcessStarted);

            _orderStateMachine.Configure(OrderStates.PaymentProcessStarted)
                              .PermitDynamic(_changePaymentStatusTrigger, SetPaymentStatus);

            _orderStateMachine.Configure(OrderStates.PaymentCompleted)
                              .OnEntry(OnPaymentCompleted)
                              .PermitReentry(OrderActions.ChangePaymentStatus)
                              .Permit(OrderActions.SetAsOrderShipped, OrderStates.OrderShipped);

            _orderStateMachine.Configure(OrderStates.PaymentFailed)
                              .OnEntry(OnPaymentFailed)
                              .Permit(OrderActions.SetAsOrderClosed, OrderStates.OrderClosed);

            _orderStateMachine.Configure(OrderStates.OrderShipped)
                              .PermitDynamic(_changeShipmentStatusTrigger, SetShipmentStatus);

            _orderStateMachine.Configure(OrderStates.ShipmentDelivered)
                              .OnEntry(OnShipmentDelivered)
                              .Permit(OrderActions.SetAsOrderCompleted, OrderStates.OrderCompleted);

            _orderStateMachine.Configure(OrderStates.ShipmentReturned)
                              .OnEntry(OnShipmentReturned)
                              .Permit(OrderActions.SetAsRefundStarted, OrderStates.RefundStarted);

            _orderStateMachine.Configure(OrderStates.RefundStarted)
                              .Permit(OrderActions.SetAsRefundCompleted, OrderStates.RefundCompleted);

            _orderStateMachine.Configure(OrderStates.RefundCompleted)
                              .OnEntry(OnRefundCompleted)
                              .Permit(OrderActions.SetAsOrderClosed, OrderStates.OrderClosed);

            _orderStateMachine.OnTransitioned(transition =>
                                              {
                                                  _orderModel.SetState((int) transition.Destination);
                                                  dataContext.SaveChanges();
                                              });
            _orderStateMachine.OnUnhandledTrigger((states, actions) => throw new InvalidStatusTransitionException(states.ToString(), actions.ToString()));
        }

        #region OnStateChanged

        private void OnSubmitted()
        {
            _integrationMessagePublisher.AddMessage(new TakePaymentCommand(_orderModel.Id.ToString(), _orderModel.TotalAmount));
        }

        private OrderStates SetPaymentStatus(PaymentStatuses paymentStatus)
        {
            return paymentStatus switch
                   {
                       PaymentStatuses.Completed => OrderStates.PaymentCompleted,
                       PaymentStatuses.Failed => OrderStates.PaymentFailed,
                       _ => throw new ArgumentOutOfRangeException(nameof(paymentStatus), paymentStatus, null)
                   };
        }

        private void OnPaymentCompleted()
        {
            _integrationMessagePublisher.AddMessage(new PrepareShipmentCommand(_orderModel.Id.ToString(), _orderModel.BuyerName, _orderModel.BuyerAddress));
        }

        private void OnPaymentFailed()
        {
            _orderStateMachine.Fire(OrderActions.SetAsOrderClosed);
        }

        private OrderStates SetShipmentStatus(ShipmentStatuses shipmentStatus)
        {
            return shipmentStatus switch
                   {
                       ShipmentStatuses.Delivered => OrderStates.ShipmentDelivered,
                       ShipmentStatuses.Returned => OrderStates.ShipmentReturned,
                       _ => throw new ArgumentOutOfRangeException(nameof(shipmentStatus), shipmentStatus, null)
                   };
        }

        private void OnShipmentDelivered()
        {
            _orderStateMachine.Fire(OrderActions.SetAsOrderCompleted);
        }

        private void OnShipmentReturned()
        {
            _integrationMessagePublisher.AddMessage(new RefundCommand(_orderModel.Id.ToString()));
        }

        private void OnRefundCompleted()
        {
            _orderStateMachine.Fire(OrderActions.SetAsOrderClosed);
        }

        #endregion

        #region InterfaceSection

        public OrderStates CurrentState => _orderStateMachine.State;
        public OrderResponse OrderResponse => _orderModel.ToOrderResponse();

        public void SubmitOrder()
        {
            _orderStateMachine.Fire(OrderActions.Submit);
        }

        public void SetAsPaymentStarted()
        {
            _orderStateMachine.Fire(OrderActions.SetAsPaymentStarted);
        }

        public void ChangePaymentStatus(PaymentStatuses paymentStatus)
        {
            _orderStateMachine.Fire(_changePaymentStatusTrigger, paymentStatus);
        }

        public void SetAsOrderShipped()
        {
            _orderStateMachine.Fire(OrderActions.SetAsOrderShipped);
        }

        public void ChangeShipmentStatus(ShipmentStatuses shipmentStatus)
        {
            _orderStateMachine.Fire(_changeShipmentStatusTrigger, shipmentStatus);
        }

        public void SetAsRefundStarted()
        {
            _orderStateMachine.Fire(OrderActions.SetAsRefundStarted);
        }

        public void SetAsRefundCompleted()
        {
            _orderStateMachine.Fire(OrderActions.SetAsRefundCompleted);
        }

        #endregion
    }
}

