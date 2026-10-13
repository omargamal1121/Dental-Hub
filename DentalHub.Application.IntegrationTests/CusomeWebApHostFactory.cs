using DentalHub.API;
using DentalHub.Infrastructure.ContextAndConfig;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DentalHub.Application.IntegrationTests
{
	public class CusomeWebApHostFactory:WebApplicationFactory<Program>
	{
		private readonly string _connectionString;
		public CusomeWebApHostFactory(string connectionstring)
		{
			_connectionString = connectionstring;
			
		}
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseSetting(
			 "ConnectionStrings:DefaultConnection",
			 _connectionString);
			builder.UseSetting(
			 "Jwt:Key",
			 "K8m#nP9$vL2@qR7!xY4*wE6%hJ3^tU8ffA5_sD1$gH9@kL4!qq7%pQ2^rT5&vW8#xZ1$bC4@dF7!gH0%jK3^lM6&nP9#qR2$sT5@uV8!wX1%yZ4^aB7&cD0#eF3$gH6@iJ9!kL2%mN5^ns8&");
			builder.UseSetting(
			 "Jwt:Issuer",
			 "this_by_the_greatest_omargamal");
			builder.UseSetting(
			 "Jwt:Audience",
			 "HTis TEst");
			builder.UseSetting(
			 "Jwt:ExpiresInMinutes",
			 "100");
			builder.UseSetting(
			 "Jwt:RefreshTokenExpiryDays",
			 "7");


			base.ConfigureWebHost(builder);

			//builder.ConfigureServices(Services =>
			//{
			//	var oldcontext = Services.SingleOrDefault(d => d.ServiceType == typeof(ContextApp));
			//	if(oldcontext != null)
			//		Services.Remove(oldcontext);

			//	Services.AddDbContext<ContextApp>(option=>
			//	{
			//		option.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
			//	});
			


			//}
			//);
		}
	}
}
