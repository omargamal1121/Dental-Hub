using DentalHub.API.Middleware;
using DentalHub.Application.Extensions;
using DentalHub.Application.Services.Auth;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.ContextAndConfig;
using DentalHub.Infrastructure.Extensions;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Serilog;

namespace DentalHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			Log.Logger = new LoggerConfiguration()
	    .MinimumLevel.Information()
	    .WriteTo.Console()
	    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
	    .CreateLogger();
			// ========== Add Controllers ==========
			builder.Services.AddControllers();
            
			builder.Host.UseSerilog();

			// ========== Add API Explorer for Swagger ==========
			builder.Services.AddEndpointsApiExplorer();
       
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
            builder.Services.AddScoped<IPasswordService, PasswordService>();
      //      builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<IAccountEmailService, AccountEmailService>();
			builder.Services.AddHangfire(config =>
			config.UseStorage(
				new MySqlStorage(
					builder.Configuration.GetConnectionString("DefaultConnection"),
					new MySqlStorageOptions
					{
						TablesPrefix = "Hangfire_",
						QueuePollInterval = TimeSpan.FromSeconds(10),
					}
				)
			)
		);

            builder.Services.AddHangfireServer();


			// ========== Configure Swagger ==========
			builder.Services.AddSwaggerGen();

            // ========== Add Infrastructure Services ==========
            // This registers: DbContext, Repositories, UnitOfWork
            builder.Services.AddInfrastructureServices(builder.Configuration);

            // ========== Add Application Services ==========
            // This registers: All your services (Auth, Patient, Student, etc.)
            builder.Services.AddApplicationServices();

            // ========== Add MediatR ==========
            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
                cfg.RegisterServicesFromAssemblies(
                    AppDomain.CurrentDomain.GetAssemblies()
                        .Where(a => a.FullName?.StartsWith("DentalHub") ?? false)
                        .ToArray()
                );
            });
      
			builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ContextApp>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
                options.LoginPath = "/api/auth/login";
                options.AccessDeniedPath = "/api/auth/access-denied";
                options.SlidingExpiration = true;
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });


            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseGlobalExceptionHandler();

          
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "DentalHub API v1");
                    options.RoutePrefix = "swagger";
                });
           

          

       
            app.UseRouting();

       
            app.UseCors("AllowAll");

     
            app.UseAuthentication();
            app.UseAuthorization();

            // Map Controllers
            app.MapControllers();
            app.UseHangfireDashboard("/hangfire");

			// Redirect root to Swagger
			app.MapGet("/", () => Results.Redirect("/swagger"));

            // Run the application
            app.Run();
        }
    }
}
