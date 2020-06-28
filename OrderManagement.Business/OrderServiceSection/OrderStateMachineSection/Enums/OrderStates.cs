namespace OrderManagement.Business.OrderServiceSection.OrderStateMachineSection.Enums
{
    public enum OrderStates
    {
        Initial = 0,
        
        PaymentProcessStarted = 1,
        PaymentCompleted = 2,
        PaymentFailed = 3,
        
        OrderShipped = 4,
        ShipmentDelivered = 5,
        ShipmentReturned = 6,
        
        RefundStarted = 7,
        RefundCompleted = 8
    }
}