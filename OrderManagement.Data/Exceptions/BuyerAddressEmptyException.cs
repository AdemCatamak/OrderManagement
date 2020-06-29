using OrderManagement.Exceptions;

namespace OrderManagement.Data.Exceptions
{
    public class BuyerAddressEmptyException : ValidationException
    {
        public BuyerAddressEmptyException() : base("Buyer address should not be empty")
        {
        }
    }
}