using DentalHub.Application.Services.Cases;
using DentalHub.Application.Services.Doctors;
using DentalHub.Application.Services.Identity;
using DentalHub.Application.Services.Patients;
using DentalHub.Application.Services.Sessions;
using DentalHub.Application.Services.Students;
using Microsoft.Extensions.DependencyInjection;

namespace DentalHub.Application
{
    /// <summary>
    /// Extension methods for registering Application layer services
    /// هنا بنسجل كل الـ Services في الـ DI Container
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Identity Services
            services.AddScoped<IUserManagementService, AuthService>();

            // Patient Services
            services.AddScoped<IPatientService, PatientService>();

            // Student Services
            services.AddScoped<IStudentService, StudentService>();

            // Doctor Services
            services.AddScoped<IDoctorService, DoctorService>();

            // Case Services
            services.AddScoped<IPatientCaseService, PatientCaseService>();
            services.AddScoped<ICaseRequestService, CaseRequestService>();

            // Session Services
            services.AddScoped<ISessionService, SessionService>();

            return services;
        }
    }
}
