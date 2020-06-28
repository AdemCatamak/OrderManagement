using OrderManagement.Business.PaymentServiceSection;
using OrderManagement.Business.ShipmentServiceSection;
using OrderManagement.Data.Models;

namespace OrderManagement.Business.OrderServiceSection.OrderStateMachineSection
{
    public class OrderStateMachineFactory : IOrderStateMachineFactory
    {
        private readonly IPaymentService _paymentService;
        private readonly IShipmentService _shipmentService;

        public OrderStateMachineFactory(IPaymentService paymentService, IShipmentService shipmentService)
        {
            _paymentService = paymentService;
            _shipmentService = shipmentService;
        }

        public IOrderStateMachine BuildOrderStateMachine(OrderModel orderModel)
        {
            var orderStateMachine = new OrderStateMachine(orderModel, _paymentService, _shipmentService);
            return orderStateMachine;
        }
    }
}