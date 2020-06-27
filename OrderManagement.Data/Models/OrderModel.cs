using System;
using Automatonymous;
using MassTransit;
using OrderManagement.Data.Models.BaseModels;

namespace OrderManagement.Data.Models
{
    public class OrderModel : SagaStateMachineInstance,
                              ICreateAudit,
                              IUpdateAudit
    {
        public Guid CorrelationId { get; set; }
        public Guid OrderId => CorrelationId;
        public DateTime CreatedOn { get; private set; }
        public DateTime UpdatedOn { get; private set; }
        public string BuyerName { get; private set; }
        public string BuyerAddress { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string OrderState { get; private set; }
        
        public byte[] RowVersion { get; private set; }

        public OrderModel(string buyerName, string buyerAddress, decimal totalAmount)
            : this(NewId.NextGuid(), DateTime.UtcNow, DateTime.UtcNow, buyerName, buyerAddress, totalAmount, "Initial", default)
        {
        }

        public OrderModel(Guid correlationId, DateTime createdOn, DateTime updatedOn, string buyerName, string buyerAddress, decimal totalAmount, string orderState, byte[] rowVersion)
        {
            CorrelationId = correlationId;
            CreatedOn = createdOn;
            UpdatedOn = updatedOn;
            BuyerName = buyerName;
            BuyerAddress = buyerAddress;
            TotalAmount = totalAmount;
            OrderState = orderState;
            RowVersion = rowVersion;
        }
    }
}