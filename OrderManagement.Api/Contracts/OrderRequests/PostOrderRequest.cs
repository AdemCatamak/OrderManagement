namespace OrderManagement.Api.Contracts.OrderRequests
{
    public class PostOrderRequest
    {
        public string BuyerName { get; set; }
        public string BuyerAddress { get; set; }
        public decimal TotalAmount { get; set; }
    }
}