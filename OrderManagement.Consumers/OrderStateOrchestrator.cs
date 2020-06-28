using System.Threading.Tasks;
using MassTransit;
using OrderManagement.Business.Events;
using OrderManagement.Business.ExternalEvents.PaymentEvents;
using OrderManagement.Business.ExternalEvents.ShipmentEvents;
using OrderManagement.Business.OrderServiceSection;
using OrderManagement.Business.OrderServiceSection.OrderStateMachineSection.Enums;
using OrderManagement.Business.OrderServiceSection.Responses;
using OrderManagement.Utility.DistributedLockSection;

namespace OrderManagement.Consumers
{
    public class OrderStateOrchestrator :
        IConsumer<OrderCreatedEvent>,
        IConsumer<PaymentCompletedEvent>,
        IConsumer<PaymentFailedEvent>,
        IConsumer<ShipmentDeliveredEvent>,
        IConsumer<ShipmentReturnedEvent>,
        IConsumer<PaymentRefundedEvent>
    {
        private readonly IOrderService _orderService;
        private readonly IDistributedLockManager _distributedLockManager;

        public OrderStateOrchestrator(IOrderService orderService, IDistributedLockManager distributedLockManager)
        {
            _orderService = orderService;
            _distributedLockManager = distributedLockManager;
        }

        private static string OrderOperationKey(long orderId) => $"OrderLockKey-{orderId}";

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            OrderCreatedEvent orderCreatedEvent = context.Message;
            OrderResponse orderResponse = orderCreatedEvent.OrderResponse;

            await _distributedLockManager.LockAsync(OrderOperationKey(orderResponse.OrderId),
                                                    async () => { await _orderService.TakePaymentAsync(orderResponse.OrderId); });
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            PaymentCompletedEvent paymentCompletedEvent = context.Message;

            await _distributedLockManager.LockAsync(OrderOperationKey(paymentCompletedEvent.OrderId),
                                                    async () => { await _orderService.ChangePaymentProcessStatusAsync(paymentCompletedEvent.OrderId, PaymentStatuses.Completed); });
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            PaymentFailedEvent paymentFailedEvent = context.Message;

            await _distributedLockManager.LockAsync(OrderOperationKey(paymentFailedEvent.OrderId),
                                                    async () => { await _orderService.ChangePaymentProcessStatusAsync(paymentFailedEvent.OrderId, PaymentStatuses.Failed); }
                                                   );
        }

        public async Task Consume(ConsumeContext<ShipmentDeliveredEvent> context)
        {
            ShipmentDeliveredEvent shipmentDeliveredEvent = context.Message;

            await _distributedLockManager.LockAsync(OrderOperationKey(shipmentDeliveredEvent.OrderId),
                                                    async () => { await _orderService.ChangeShipmentStatusAsync(shipmentDeliveredEvent.OrderId, ShipmentStatuses.Delivered); }
                                                   );
        }

        public async Task Consume(ConsumeContext<ShipmentReturnedEvent> context)
        {
            ShipmentReturnedEvent shipmentReturnedEvent = context.Message;

            await _distributedLockManager.LockAsync(OrderOperationKey(shipmentReturnedEvent.OrderId),
                                                    async () => { await _orderService.ChangeShipmentStatusAsync(shipmentReturnedEvent.OrderId, ShipmentStatuses.Returned); }
                                                   );
        }

        public async Task Consume(ConsumeContext<PaymentRefundedEvent> context)
        {
            PaymentRefundedEvent paymentRefundedEvent = context.Message;

            await _distributedLockManager.LockAsync(OrderOperationKey(paymentRefundedEvent.OrderId),
                                                    async () => { await _orderService.SetRefundCompletedAsync(paymentRefundedEvent.OrderId); }
                                                   );
        }
    }
}