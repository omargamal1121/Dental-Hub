using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.ContextAndConfig;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DentalHub.Application.IntegrationTests
{
	public static class UserMangerFactory
	{
		public static UserManager<User> CreateRealUserManager(ContextApp context)
		{
			var store = new UserStore<User, IdentityRole<Guid>, ContextApp, Guid>(context);
			var options = Options.Create(new IdentityOptions());
			var passwordHasher = new PasswordHasher<User>();
			var userValidators = new List<IUserValidator<User>> { new UserValidator<User>() };
			var passwordValidators = new List<IPasswordValidator<User>> { new PasswordValidator<User>() };
			var keyNormalizer = new UpperInvariantLookupNormalizer();
			var errors = new IdentityErrorDescriber();
			var services = new ServiceCollection().BuildServiceProvider();

			return new UserManager<User>(
				store, options, passwordHasher, userValidators,
				passwordValidators, keyNormalizer, errors, services, null!);
		}
		public static async Task SeedRolesAsync(ContextApp context)
		{
			var roleStore = new RoleStore<IdentityRole<Guid>, ContextApp, Guid>(context);
			var roleManager = new RoleManager<IdentityRole<Guid>>(
				roleStore,
				new List<IRoleValidator<IdentityRole<Guid>>> { new RoleValidator<IdentityRole<Guid>>() },
				new UpperInvariantLookupNormalizer(),
				new IdentityErrorDescriber(),
				null!);

			string[] roles = { "Patient", "Doctor", "Student", "Admin" };

			foreach (var role in roles)
			{
				if (!await roleManager.RoleExistsAsync(role))
				{
					await roleManager.CreateAsync(new IdentityRole<Guid>(role));
				}
			}
		}
	}
}
