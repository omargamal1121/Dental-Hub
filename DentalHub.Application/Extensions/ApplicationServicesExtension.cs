using DentalHub.Application.Services.Admins;
using DentalHub.Application.Services.Cases;
using DentalHub.Application.Services.Doctors;
using DentalHub.Application.Services.Identity;
using DentalHub.Application.Services.Patients;
using DentalHub.Application.Services.Sessions;
using DentalHub.Application.Services.Students;
using Microsoft.Extensions.DependencyInjection;

namespace DentalHub.Application.Extensions
{
    /// Extension methods for registering application services
    public static class ApplicationServicesExtension
    {
        /// Register all application services to the DI container
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services)
        {
            // Register Authentication & Identity Services
            services.AddScoped<IUserManagementService, AuthService>();

            // Register User Management Services
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IAdminService, AdminService>();

            // Register Case Management Services
            services.AddScoped<IPatientCaseService, PatientCaseService>();
            services.AddScoped<ICaseRequestService, CaseRequestService>();

            // Register Session Management Services
            services.AddScoped<ISessionService, SessionService>();

            return services;
        }
    }
}
