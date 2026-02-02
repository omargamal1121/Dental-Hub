namespace DentalHub.Application.Exceptions
{
    /// <summary>
    /// Exception thrown when a business rule is violated
    /// مثال: الطالب عنده session في نفس الوقت
    /// </summary>
    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message)
            : base(message)
        {
        }

        public BusinessRuleException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
