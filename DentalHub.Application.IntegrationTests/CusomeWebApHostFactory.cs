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
