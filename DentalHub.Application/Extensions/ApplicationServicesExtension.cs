using DentalHub.Application.Services.Cases;
using DentalHub.Application.Services.Doctors;
using DentalHub.Application.Services.Identity;
using DentalHub.Application.Services.Patients;
using DentalHub.Application.Services.Sessions;
using DentalHub.Application.Services.Students;
using Microsoft.Extensions.DependencyInjection;

namespace DentalHub.Application.Extensions
{
    /// <summary>
    /// Extension methods for registering application services
    /// </summary>
    public static class ApplicationServicesExtension
    {
        /// <summary>
        /// Register all application services to the DI container
        /// </summary>
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // Register Authentication & Identity Services
            services.AddScoped<IUserManagementService, AuthService>();

            // Register User Management Services
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IDoctorService, DoctorService>();

            // Register Case Management Services
            services.AddScoped<IPatientCaseService, PatientCaseService>();
            services.AddScoped<ICaseRequestService, CaseRequestService>();

            // Register Session Management Services
            services.AddScoped<ISessionService, SessionService>();

            return services;
        }
    }
}
