using OrderManagement.Exceptions;

namespace OrderManagement.Business.Pagination.Exceptions
{
    public class OffsetInvalidException : ValidationException
    {
        public OffsetInvalidException() : base("Invalid offset value")
        {
        }
    }
}