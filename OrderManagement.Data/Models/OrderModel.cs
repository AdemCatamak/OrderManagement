using System;
using OrderManagement.Data.Enum;
using OrderManagement.Data.Models.BaseModels;

namespace OrderManagement.Data.Models
{
    public class OrderModel : IEntity<long>,
                              ICreateAudit,
                              IUpdateAudit
    {
        public long Id { get; private set; }
        public DateTime CreatedOn { get; private set; }
        public DateTime UpdatedOn { get; private set; }
        public string BuyerName { get; private set; }
        public string BuyerAddress { get; private set; }
        public decimal TotalAmount { get; private set; }
        public OrderStates OrderState { get; private set; }

        public OrderModel(string buyerName, string buyerAddress, decimal totalAmount)
            : this(default, DateTime.UtcNow, DateTime.UtcNow, buyerAddress, buyerAddress, totalAmount, OrderStates.OrderCreated)
        {
        }

        public OrderModel(long id, DateTime createdOn, DateTime updatedOn, string buyerName, string buyerAddress, decimal totalAmount, OrderStates orderState)
        {
            Id = id;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
            BuyerName = buyerName;
            BuyerAddress = buyerAddress;
            TotalAmount = totalAmount;
            OrderState = orderState;
        }
    }
}