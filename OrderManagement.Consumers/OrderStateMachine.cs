using Automatonymous;
using OrderManagement.Business.BillingServiceSection;
using OrderManagement.Business.Events;
using OrderManagement.Business.ExternalEvents.BillingEvents;
using OrderManagement.Business.ExternalEvents.PaymentEvents;
using OrderManagement.Business.ExternalEvents.ShipmentEvents;
using OrderManagement.Business.OrderServiceSection;
using OrderManagement.Business.OrderServiceSection.Responses;
using OrderManagement.Business.PaymentServiceSection;
using OrderManagement.Business.ShipmentServiceSection;
using OrderManagement.Data.Models;

namespace OrderManagement.Consumers
{
    public class OrderStateMachine :
        MassTransitStateMachine<OrderModel>
    {
        public OrderStateMachine(IOrderService orderService,
                                 IPaymentService paymentService,
                                 IBillingService billingService,
                                 IShipmentService shipmentService)
        {
            InstanceState(x => x.OrderState);

            Event(() => OrderCreated, x => x.CorrelateById(context => context.Message.OrderResponse.OrderId));
            Event(() => PaymentCompleted, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => PaymentFailed, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => BillingCompleted, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => ShipmentDelivered, x => x.CorrelateById(context => context.Message.OrderId));
            Event(() => ShipmentReturned, x => x.CorrelateById(context => context.Message.OrderId));

            During(OrderCreatedState,
                   When(OrderCreated)
                      .ThenAsync(async context =>
                                 {
                                     OrderResponse orderResponse = context.Data.OrderResponse;
                                     await paymentService.TakePaymentAsync(orderResponse.OrderId.ToString(), orderResponse.TotalAmount);
                                 })
                      .TransitionTo(PaymentProcessTriggeredState));

            During(PaymentProcessTriggeredState,
                   When(PaymentFailed)
                      .TransitionTo(PaymentProcessFailedState)
                  );

            During(PaymentProcessTriggeredState,
                   When(PaymentCompleted)
                      .TransitionTo(PaymentProcessCompletedState)
                      .ThenAsync(async context =>
                                 {
                                     PaymentProcessCompletedEvent paymentProcessCompletedEvent = context.Data;
                                     OrderResponse orderResponse = await orderService.GetOrderAsync(paymentProcessCompletedEvent.OrderId);
                                     await billingService.CreateBill(orderResponse.OrderId.ToString(), orderResponse.TotalAmount, orderResponse.BuyerName);
                                 })
                      .TransitionTo(BillingProcessTriggeredState)
                  );

            During(BillingProcessTriggeredState,
                   When(BillingCompleted)
                      .TransitionTo(BillingProcessCompletedState)
                      .ThenAsync(async context =>
                                 {
                                     BillingProcessCompletedEvent billingProcessCompletedEvent = context.Data;
                                     OrderResponse orderResponse = await orderService.GetOrderAsync(billingProcessCompletedEvent.OrderId);
                                     await shipmentService.CreateShipmentAsync(orderResponse.OrderId.ToString(),orderResponse.BuyerName, orderResponse.BuyerAddress);
                                 })
                      .TransitionTo(ShipmentProcessTriggeredState)
                  );

            During(ShipmentProcessTriggeredState,
                   When(ShipmentDelivered)
                      .TransitionTo(ShipmentDeliveredState));

            During(ShipmentProcessTriggeredState,
                   When(ShipmentReturned)
                      .ThenAsync(async context =>
                                 {
                                     ShipmentReturnedEvent shipmentReturnedEvent = context.Data;
                                     OrderResponse orderResponse = await orderService.GetOrderAsync(shipmentReturnedEvent.OrderId);

                                     await billingService.CancelBillAsync(orderResponse.OrderId.ToString());
                                 })
                      .ThenAsync(async context =>
                                 {
                                     ShipmentReturnedEvent shipmentReturnedEvent = context.Data;
                                     OrderResponse orderResponse = await orderService.GetOrderAsync(shipmentReturnedEvent.OrderId);

                                     await paymentService.RefundAsync(orderResponse.OrderId.ToString());
                                 })
                      .TransitionTo(ShipmentReturnedState));
        }

        #region States

        public State OrderCreatedState { get; set; }

        public State PaymentProcessTriggeredState { get; set; }
        public State PaymentProcessFailedState { get; set; }
        public State PaymentProcessCompletedState { get; set; }

        public State BillingProcessTriggeredState { get; set; }
        public State BillingProcessCompletedState { get; set; }

        public State ShipmentProcessTriggeredState { get; set; }
        public State ShipmentDeliveredState { get; set; }
        public State ShipmentReturnedState { get; set; }

        #endregion

        #region Events

        public Event<OrderCreatedEvent> OrderCreated { get; private set; }

        public Event<PaymentProcessCompletedEvent> PaymentCompleted { get; private set; }
        public Event<PaymentProcessFailedEvent> PaymentFailed { get; private set; }

        public Event<BillingProcessCompletedEvent> BillingCompleted { get; private set; }

        public Event<ShipmentDeliveredEvent> ShipmentDelivered { get; private set; }
        public Event<ShipmentReturnedEvent> ShipmentReturned { get; private set; }

        #endregion
    }
}