using DentalHub.Application.Interfaces;
using DentalHub.Application.Services.Admins;
using DentalHub.Application.Services.Cases;
using DentalHub.Application.Services.CaseTypes;
using DentalHub.Application.Services.DiagnosesServices;
using DentalHub.Application.Services.Doctors;
using DentalHub.Application.Services.Identity;
using DentalHub.Application.Services;
using DentalHub.Application.Services.Sessions;
using DentalHub.Application.Services.Students;
using DentalHub.Application.Services.UniversityMembers;
using FluentValidation;
using MediatR;
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
            services.AddScoped<IUserManagementService, UserManagementService>();

            // Register User Management Services
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IAdminService, AdminService>();

            // Register Case Management Services
            services.AddScoped<IPatientCaseService, PatientCaseService>();
            services.AddScoped<ICaseRequestService, CaseRequestService>();
            services.AddScoped<ICaseTypeService, CaseTypeService>();
            services.AddScoped<IDiagnosisService, DiagnosisService>();
          

            // Register Session Management Services
            services.AddScoped<ISessionService, SessionService>();

            // Register University Member Service
            services.AddScoped<IUniversityMemberService, UniversityMemberService>();

            // Register FluentValidation
            services.AddValidatorsFromAssembly(typeof(ApplicationServicesExtension).Assembly);

            // Register MediatR Pipeline Behaviors
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(DentalHub.Application.Common.Behaviors.ValidationBehavior<,>));

            return services;
        }
    }
}
