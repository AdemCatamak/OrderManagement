using OrderManagement.Exceptions;

namespace OrderManagement.Business.Pagination.Exceptions
{
    public class TakeInvalidException: ValidationException
    {
        public TakeInvalidException() : base(ExceptionMessages.TAKE_INVALID)
        {
        }
    }
}