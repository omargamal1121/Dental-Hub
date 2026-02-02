namespace DentalHub.Application.Exceptions
{
    /// <summary>
    /// Exception thrown when a requested entity is not found
    /// </summary>
    public class NotFoundException : Exception
    {
        public NotFoundException(string entityName, object key)
            : base($"{entityName} with id '{key}' was not found.")
        {
        }

        public NotFoundException(string message)
            : base(message)
        {
        }
    }
}
