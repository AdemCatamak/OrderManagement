using System;
using OrderManagement.Exceptions;

namespace OrderManagement.Business.OrderServiceSection.Exceptions
{
    public class OrderNotFoundException : NotFoundException
    {
        public OrderNotFoundException() : base($"Order not found")
        {
        }
        public OrderNotFoundException(Guid orderId) : base($"Order not found with given id {orderId}")
        {
        }
    }
}