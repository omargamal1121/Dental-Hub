using DentalHub.Infrastructure.ContextAndConfig;
using Microsoft.EntityFrameworkCore;
using System;

namespace DentalHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddDbContext<ContextApp>(options =>
	        options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
					   new MySqlServerVersion(new Version(8, 0, 33)))
            );

			builder.Services.AddSwaggerGen();

			var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
         
                app.UseSwagger();
                app.UseSwaggerUI();
			}

            app.UseRouting();
         
			app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
