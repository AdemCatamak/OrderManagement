using OrderManagement.Exceptions;

namespace OrderManagement.Business.OrderServiceSection.Exceptions
{
    public class BuyerAddressEmptyException : ValidationException
    {
        public BuyerAddressEmptyException() : base("Buyer address should not be empty")
        {
        }
    }
}