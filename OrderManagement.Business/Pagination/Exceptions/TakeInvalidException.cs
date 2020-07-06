using OrderManagement.Exceptions;

namespace OrderManagement.Business.Pagination.Exceptions
{
    public class TakeInvalidException : ValidationException
    {
        public TakeInvalidException() : base("Invalid take value")
        {
        }
    }
}