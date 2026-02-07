namespace DentalHub.Application.Exceptions
{
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException(string message)
            : base(message)
        {
        }

        public ForbiddenAccessException()
            : base("You are not authorized to perform this action.")
        {
        }
    }
}
