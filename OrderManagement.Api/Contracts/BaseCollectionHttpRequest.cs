namespace OrderManagement.Api.Contracts
{
    public abstract class BaseCollectionHttpRequest
    {
        public int Offset { get; set; } = 0;
        public int Take { get; set; } = 1;
    }
}