using System.Collections.Generic;

namespace OrderManagement.Business.Domain.OrderServiceSection.Responses
{
    public class OrderCollectionResponse
    {
        public OrderCollectionResponse(int totalCount, List<OrderResponse> data)
        {
            TotalCount = totalCount;
            Data = data;
        }

        public int TotalCount { get; }
        public List<OrderResponse> Data { get; }
    }
}