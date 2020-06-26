using System;

namespace OrderManagement.Business.OrderServiceSection.Responses
{
    public class OrderResponse
    {
        public long Id { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public DateTime UpdatedOn { get; private set; }
        public string BuyerName { get; private set; }
        public string BuyerAddress { get; private set; }
        public decimal TotalAmount { get; private set; }

        public OrderResponse(long id, DateTime createdOn, DateTime updatedOn, string buyerName, string buyerAddress, decimal totalAmount)
        {
            Id = id;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
            BuyerName = buyerName;
            BuyerAddress = buyerAddress;
            TotalAmount = totalAmount;
        }
    }
}