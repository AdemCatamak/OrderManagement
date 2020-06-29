using OrderManagement.Exceptions;

namespace OrderManagement.Data.Exceptions
{
    public class OrderAmountNegativeException : ValidationException
    {
        public OrderAmountNegativeException(decimal totalAmount) : base($"Total amount should be greater than or equal to zero [{totalAmount}]")
        {
        }
    }
}