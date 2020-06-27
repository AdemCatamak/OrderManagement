using System;

namespace OrderManagement.Business.OrderServiceSection.Responses
{
    public class OrderResponse
    {
        public Guid OrderId { get; }
        public DateTime CreatedOn { get; }
        public DateTime UpdatedOn { get; }
        public string BuyerName { get; }
        public string BuyerAddress { get; }
        public decimal TotalAmount { get; }

        public OrderResponse(Guid orderId, DateTime createdOn, DateTime updatedOn, string buyerName, string buyerAddress, decimal totalAmount)
        {
            OrderId = orderId;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
            BuyerName = buyerName;
            BuyerAddress = buyerAddress;
            TotalAmount = totalAmount;
        }
    }
}