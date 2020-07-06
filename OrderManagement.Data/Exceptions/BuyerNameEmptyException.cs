using OrderManagement.Exceptions;

namespace OrderManagement.Data.Exceptions
{
    public class BuyerNameEmptyException : ValidationException
    {
        public BuyerNameEmptyException() : base("Buyer name should not be empty")
        {
        }
    }
}