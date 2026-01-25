using Microsoft.AspNetCore.Identity;

namespace DentalHub.Domain.Entities
{
	public class User<T>:IdentityUser<T> where T:IEquatable<T>
	{

	}
}
