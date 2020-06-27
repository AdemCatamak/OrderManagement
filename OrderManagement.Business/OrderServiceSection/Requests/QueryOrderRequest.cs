using System;

namespace OrderManagement.Business.OrderServiceSection.Requests
{
    public class QueryOrderRequest
    {
        public int Offset { get; }
        public int Take { get; }

        public QueryOrderRequest(in int offset, in int take)
        {
            Offset = offset;
            Take = take;
        }

        public Guid? OrderId { get; set; }
    }
}