using System;
using System.Threading.Tasks;
using MassTransit;
using OrderManagement.Business.Clients;
using OrderManagement.Business.Commands;
using OrderManagement.Business.Domain.OrderStateMachineSection;
using OrderManagement.Business.Domain.OrderStateMachineSection.Enums;
using OrderManagement.Business.ExternalEvents.PaymentEvents;
using OrderManagement.Business.ExternalEvents.ShipmentEvents;
using OrderManagement.Utility.DistributedLockSection;
using OrderManagement.Utility.IntegrationMessagePublisherSection;

namespace OrderManagement.Consumers
{
    public class OrderStateOrchestrator :
        IConsumer<TakePaymentCommand>,
        IConsumer<PaymentCreatedEvent>,
        IConsumer<PaymentCompletedEvent>,
        IConsumer<PaymentFailedEvent>,
        IConsumer<PrepareShipmentCommand>,
        IConsumer<ShipmentCreatedEvent>,
        IConsumer<ShipmentDeliveredEvent>,
        IConsumer<ShipmentReturnedEvent>,
        IConsumer<RefundCommand>,
        IConsumer<RefundStartedEvent>,
        IConsumer<RefundCompletedEvent>
    {
        private readonly IPaymentServiceClient _paymentServiceClient;
        private readonly IDistributedLockManager _distributedLockManager;
        private readonly IOrderStateMachineFactory _orderStateMachineFactory;
        private readonly IShipmentServiceClient _shipmentServiceClient;
        private readonly IIntegrationMessagePublisher _integrationMessagePublisher;

        public OrderStateOrchestrator(IPaymentServiceClient paymentServiceClient,
                                      IDistributedLockManager distributedLockManager,
                                      IOrderStateMachineFactory orderStateMachineFactory,
                                      IShipmentServiceClient shipmentServiceClient,
                                      IIntegrationMessagePublisher integrationMessagePublisher)
        {
            _distributedLockManager = distributedLockManager;
            _orderStateMachineFactory = orderStateMachineFactory;
            _shipmentServiceClient = shipmentServiceClient;
            _integrationMessagePublisher = integrationMessagePublisher;
            _paymentServiceClient = paymentServiceClient;
        }

        private static string OrderOperationKey(long orderId) => $"OrderLockKey-{orderId}";

        public async Task Consume(ConsumeContext<TakePaymentCommand> context)
        {
            TakePaymentCommand takePaymentCommand = context.Message;
            await _paymentServiceClient.TakePaymentAsync(takePaymentCommand.CorrelationId, takePaymentCommand.TotalAmount);
        }

        public async Task Consume(ConsumeContext<PaymentCreatedEvent> context)
        {
            PaymentCreatedEvent paymentCreatedEvent = context.Message;
            long orderId = long.Parse(paymentCreatedEvent.CorrelationId);

            await _distributedLockManager.LockAsync(OrderOperationKey(orderId),
                                                    async () =>
                                                    {
                                                        IOrderStateMachine orderStateMachine = await _orderStateMachineFactory.BuildOrderStateMachineAsync(orderId);
                                                        orderStateMachine.SetAsPaymentStarted();
                                                    });
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            PaymentCompletedEvent paymentCompletedEvent = context.Message;
            long orderId = long.Parse(paymentCompletedEvent.CorrelationId);

            await _distributedLockManager.LockAsync(OrderOperationKey(orderId),
                                                    async () =>
                                                    {
                                                        IOrderStateMachine orderStateMachine = await _orderStateMachineFactory.BuildOrderStateMachineAsync(orderId);
                                                        orderStateMachine.ChangePaymentStatus(PaymentStatuses.Completed);
                                                        Console.WriteLine($"Consumer - {nameof(PaymentCompletedEvent)} : {_integrationMessagePublisher.IntegrationMessages.Count}");
                                                    });
        }


        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            PaymentFailedEvent paymentFailedEvent = context.Message;
            long orderId = long.Parse(paymentFailedEvent.CorrelationId);

            await _distributedLockManager.LockAsync(OrderOperationKey(orderId),
                                                    async () =>
                                                    {
                                                        IOrderStateMachine orderStateMachine = await _orderStateMachineFactory.BuildOrderStateMachineAsync(orderId);
                                                        orderStateMachine.ChangePaymentStatus(PaymentStatuses.Failed);
                                                    }
                                                   );
        }

        public async Task Consume(ConsumeContext<PrepareShipmentCommand> context)
        {
            PrepareShipmentCommand prepareShipmentCommand = context.Message;

            await _shipmentServiceClient.CreateShipmentAsync(prepareShipmentCommand.CorrelationId, prepareShipmentCommand.ReceiverName, prepareShipmentCommand.ReceiverAddress);
        }

        public async Task Consume(ConsumeContext<ShipmentCreatedEvent> context)
        {
            ShipmentCreatedEvent shipmentCreatedEvent = context.Message;
            long orderId = long.Parse(shipmentCreatedEvent.CorrelationId);

            await _distributedLockManager.LockAsync(OrderOperationKey(orderId),
                                                    async () =>
                                                    {
                                                        IOrderStateMachine orderStateMachine = await _orderStateMachineFactory.BuildOrderStateMachineAsync(orderId);
                                                        orderStateMachine.SetAsOrderShipped();
                                                    }
                                                   );
        }

        public async Task Consume(ConsumeContext<ShipmentDeliveredEvent> context)
        {
            ShipmentDeliveredEvent shipmentDeliveredEvent = context.Message;
            long orderId = long.Parse(shipmentDeliveredEvent.CorrelationId);

            await _distributedLockManager.LockAsync(OrderOperationKey(orderId),
                                                    async () =>
                                                    {
                                                        IOrderStateMachine orderStateMachine = await _orderStateMachineFactory.BuildOrderStateMachineAsync(orderId);
                                                        orderStateMachine.ChangeShipmentStatus(ShipmentStatuses.Delivered);
                                                    }
                                                   );
        }

        public async Task Consume(ConsumeContext<ShipmentReturnedEvent> context)
        {
            ShipmentReturnedEvent shipmentReturnedEvent = context.Message;
            long orderId = long.Parse(shipmentReturnedEvent.CorrelationId);

            await _distributedLockManager.LockAsync(OrderOperationKey(orderId),
                                                    async () =>
                                                    {
                                                        IOrderStateMachine orderStateMachine = await _orderStateMachineFactory.BuildOrderStateMachineAsync(orderId);
                                                        orderStateMachine.ChangeShipmentStatus(ShipmentStatuses.Returned);
                                                    }
                                                   );
        }

        public async Task Consume(ConsumeContext<RefundCommand> context)
        {
            RefundCommand refundCommand = context.Message;

            await _paymentServiceClient.RefundAsync(refundCommand.CorrelationId);
        }

        public async Task Consume(ConsumeContext<RefundStartedEvent> context)
        {
            RefundStartedEvent refundStartedEvent = context.Message;
            long orderId = long.Parse(refundStartedEvent.CorrelationId);

            await _distributedLockManager.LockAsync(OrderOperationKey(orderId),
                                                    async () =>
                                                    {
                                                        IOrderStateMachine orderStateMachine = await _orderStateMachineFactory.BuildOrderStateMachineAsync(orderId);
                                                        orderStateMachine.SetAsRefundStarted();
                                                    }
                                                   );
        }

        public async Task Consume(ConsumeContext<RefundCompletedEvent> context)
        {
            var refundCompletedEvent = context.Message;
            long orderId = long.Parse(refundCompletedEvent.CorrelationId);

            await _distributedLockManager.LockAsync(OrderOperationKey(orderId),
                                                    async () =>
                                                    {
                                                        IOrderStateMachine orderStateMachine = await _orderStateMachineFactory.BuildOrderStateMachineAsync(orderId);
                                                        orderStateMachine.SetAsRefundCompleted();
                                                    }
                                                   );
        }
    }
}