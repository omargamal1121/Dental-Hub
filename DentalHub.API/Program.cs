using DentalHub.Application.Extensions;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.ContextAndConfig;
using DentalHub.Infrastructure.Extensions;
using DentalHub.API.Middleware;
using Microsoft.AspNetCore.Identity;

namespace DentalHub.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ========== Add Controllers ==========
            builder.Services.AddControllers();

            // ========== Add API Explorer for Swagger ==========
            builder.Services.AddEndpointsApiExplorer();

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

            // ========== Configure Identity ==========
            builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ContextApp>()
            .AddDefaultTokenProviders();

            // ========== Configure Cookie Settings ==========
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromHours(24);
                options.LoginPath = "/api/auth/login";
                options.AccessDeniedPath = "/api/auth/access-denied";
                options.SlidingExpiration = true;
            });

            // ========== Configure CORS ==========
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Build the application
            var app = builder.Build();

            // ========== Configure the HTTP request pipeline ==========

            // Global Exception Handler (MUST be first!)
            app.UseGlobalExceptionHandler();

            // Enable Swagger in Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "DentalHub API v1");
                    options.RoutePrefix = "swagger";
                });
            }

            // HTTPS Redirection
            app.UseHttpsRedirection();

            // Routing
            app.UseRouting();

            // CORS (Must be after UseRouting and before UseAuthorization)
            app.UseCors("AllowAll");

            // Authentication & Authorization (Order matters!)
            app.UseAuthentication();
            app.UseAuthorization();

            // Map Controllers
            app.MapControllers();

            // Redirect root to Swagger
            app.MapGet("/", () => Results.Redirect("/swagger"));

            // Run the application
            app.Run();
        }
    }
}
