namespace OrderManagement.Data.Enum
{
    public enum OrderStates
    {
        OrderCreated = 1,
        
        PaymentProcessTriggered,
        PaymentProcessCompleted,
        PaymentProcessFailed,
        
        StockControlTriggered,
        StockControlCompleted,
        StockControlFailed,
        
        BillingTriggered,
        BillingCompleted,
        
        ShipmentProcessTriggered,
        ShipmentCompleted,
        ShipmentReturned,
        
        RefundProcessTriggered,
        RefundProcessCompleted,
        RefundProcessFailed
    }
}