using System;
using OrderManagement.Business.Domain.OrderStateMachineSection.Enums;

namespace OrderManagement.Business.Domain.OrderServiceSection.Responses
{
    public class OrderResponse
    {
        public long OrderId { get; }
        public DateTime CreatedOn { get; }
        public DateTime UpdatedOn { get; }
        public string BuyerName { get; }
        public string BuyerAddress { get; }
        public decimal TotalAmount { get; }
        public OrderStates OrderState { get; }

        public OrderResponse(long orderId, DateTime createdOn, DateTime updatedOn, string buyerName, string buyerAddress, decimal totalAmount, int orderState)
        {
            OrderId = orderId;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
            BuyerName = buyerName;
            BuyerAddress = buyerAddress;
            TotalAmount = totalAmount;
            OrderState = (OrderStates) orderState;
        }
    }
}