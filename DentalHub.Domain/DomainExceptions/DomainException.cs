namespace DentalHub.Domain.DomainExceptions
{
	public class DomainException:Exception
	{
		public DomainException(string message,Exception ?exception=null):base(message,exception)
		{
			
		}
		
	}
}
