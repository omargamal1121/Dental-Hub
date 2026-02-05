using DentalHub.Application.Services.Cases;
using DentalHub.Application.Services.Doctors;
using DentalHub.Application.Services.Identity;
using DentalHub.Application.Services.Patients;
using DentalHub.Application.Services.Sessions;
using DentalHub.Application.Services.Students;
using Microsoft.Extensions.DependencyInjection;

namespace DentalHub.Application
{
 
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
           
            services.AddScoped<IUserManagementService, AuthService>();

        
            services.AddScoped<IPatientService, PatientService>();

        
            services.AddScoped<IStudentService, StudentService>();

      
            services.AddScoped<IDoctorService, DoctorService>();


            services.AddScoped<IPatientCaseService, PatientCaseService>();
            services.AddScoped<ICaseRequestService, CaseRequestService>();

            services.AddScoped<ISessionService, SessionService>();

            return services;
        }
    }
}
