namespace DentalHub.Application.Exceptions
{
    /// <summary>
    /// Exception thrown when user tries to access unauthorized resource
    /// مثال: طالب بيحاول يشوف case مش بتاعه
    /// </summary>
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
