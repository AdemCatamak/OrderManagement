namespace OrderManagement.Business.OrderServiceSection.Requests
{
    public class QueryOrderRequest
    {
        public int Offset { get; }
        public int Take { get; }
        public long? OrderId { get; set; }

        public QueryOrderRequest(in int offset, in int take)
        {
            Offset = offset;
            Take = take;
        }
    }
}