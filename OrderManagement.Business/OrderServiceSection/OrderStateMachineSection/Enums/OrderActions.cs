namespace OrderManagement.Business.OrderServiceSection.OrderStateMachineSection.Enums
{
    public enum OrderActions
    {
        TakePayment = 1,
        ChangePaymentStatus,

        PrepareShipment,
        ChangeShipmentStatus,

        Refund,
        SetRefundCompleted,
    }
}