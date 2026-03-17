using DentalHub.API.Middleware;
using DentalHub.Application.Extensions;
using DentalHub.Application.Interfaces;
using DentalHub.Application.Services.Auth;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Configurations;
using DentalHub.Infrastructure.ContextAndConfig;
using DentalHub.Infrastructure.Extensions;
using DentalHub.Infrastructure.Services;
using Hangfire;
using Hangfire.MySql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using Serilog;

namespace DentalHub.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddOpenApi();

			Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            // ========== Add Controllers ==========
            // Added default produces/consumes so Swagger shows application/json instead of text/plain
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add(new Microsoft.AspNetCore.Mvc.ProducesAttribute("application/json"));
                options.Filters.Add(new Microsoft.AspNetCore.Mvc.ConsumesAttribute("application/json"));
            });

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

            builder.Services.Configure<CloudinarySettings>(
                builder.Configuration.GetSection("CloudinarySettings")
                );
            builder.Services.AddScoped<IMediaService, MediaService>();

            builder.Services.AddHangfireServer();

            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "DentalHub API",
                    Version = "v1",
                    Description = "API Documentation for DentalHub Project"
                });

                // Required for [SwaggerResponse] annotations on endpoints
                options.EnableAnnotations();

                // Include XML comments for Swagger documentation
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);

                // JWT Authentication Support
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter JWT token like this: Bearer {your token}"
                });

                // Add security requirement correctly
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });


            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddApplicationServices();


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
                options.User.RequireUniqueEmail = false;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ContextApp>()
            .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;

            }
              ).AddJwtBearer(option =>

              {
                  option.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateLifetime = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = builder.Configuration["Jwt:Issuer"],
                      ValidAudience = builder.Configuration["Jwt:Audience"],
                      IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                          System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured"))
                      ),
                      ClockSkew = TimeSpan.Zero
                  };

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
            app.MapOpenApi();
            app.MapScalarApiReference();

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

            // ── Seed initial data ─────────────────────────────────────────────────
            await DataSeeder.SeedAsync(app.Services);

            // Run the application
            app.Run();
        }
    }
}
