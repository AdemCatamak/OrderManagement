namespace OrderManagement.Business.Domain.OrderStateMachineSection.Enums
{
    public enum OrderActions
    {
        Submit = 10,

        SetAsPaymentStarted = 20,
        ChangePaymentStatus = 21,

        SetAsOrderShipped = 30,
        ChangeShipmentStatus = 31,

        SetAsRefundStarted = 40,
        SetAsRefundCompleted = 41
    }
}