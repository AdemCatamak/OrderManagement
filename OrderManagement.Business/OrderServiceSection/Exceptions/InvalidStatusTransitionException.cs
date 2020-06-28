using OrderManagement.Exceptions;

namespace OrderManagement.Business.OrderServiceSection.Exceptions
{
    public class InvalidStatusTransitionException : ConflictException
    {
        public InvalidStatusTransitionException(string state, string action) : base($"{action} could not executed in {state}")
        {
        }
    }
}