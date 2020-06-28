namespace OrderManagement.Api.Contracts.OrderRequests
{
    public class GetOrdersRequest
    {
        public int Offset { get; set; } = 0;
        public int Take { get; set; } = 5;
        
        public long? OrderId { get; set; }
    }
}