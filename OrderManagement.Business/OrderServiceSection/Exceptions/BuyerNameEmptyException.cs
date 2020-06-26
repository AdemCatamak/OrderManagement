using OrderManagement.Exceptions;

namespace OrderManagement.Business.OrderServiceSection.Exceptions
{
    public class BuyerNameEmptyException : ValidationException
    {
        public BuyerNameEmptyException() : base("Buyer name should not be empty")
        {
        }
    }
}