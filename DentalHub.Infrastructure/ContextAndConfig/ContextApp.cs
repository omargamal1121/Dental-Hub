using DentalHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DentalHub.Infrastructure.ContextAndConfig
{
	public class ContextApp:IdentityDbContext<User<Guid>,IdentityRole<Guid>, Guid>
	{
	
		public ContextApp(DbContextOptions<ContextApp> options) : base(options)
		{
		}
	}
}
