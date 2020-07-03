namespace OrderManagement.Business.Domain.OrderStateMachineSection.Enums
{
    public enum OrderStates
    {
        Initial = 0,

        Submitted = 10,

        PaymentProcessStarted = 20,
        PaymentCompleted = 21,
        PaymentFailed = 22,

        OrderShipped = 30,
        ShipmentDelivered = 31,
        ShipmentReturned = 32,

        RefundStarted = 40,
        RefundCompleted = 41,
        
        OrderClosed = 50,
        OrderCompleted = 60
    }
}