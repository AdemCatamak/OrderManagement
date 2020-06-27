using OrderManagement.Business.OrderServiceSection.Exceptions;

namespace OrderManagement.Business.OrderServiceSection.Requests
{
    public class CreateOrderRequest 
    {
        public string BuyerName { get; }
        public string BuyerAddress { get; }
        public decimal TotalAmount { get; }

        public CreateOrderRequest(string buyerName, string buyerAddress, decimal totalAmount)
        {
            buyerName = buyerName?.Trim() ?? string.Empty;
            buyerAddress = buyerAddress?.Trim() ?? string.Empty;

            if (buyerName == string.Empty) throw new BuyerNameEmptyException();
            BuyerName = buyerName;

            if (buyerAddress == string.Empty) throw new BuyerAddressEmptyException();
            BuyerAddress = buyerAddress;

            if (totalAmount < decimal.Zero) throw new OrderAmountNegativeException(totalAmount);
            TotalAmount = totalAmount;
        }
    }
}