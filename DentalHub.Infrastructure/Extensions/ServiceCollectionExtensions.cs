using DentalHub.Application.Interfaces;
using DentalHub.Infrastructure.ContextAndConfig;
using DentalHub.Infrastructure.Repository;
using DentalHub.Infrastructure.Repository.PatientCaseRepo;
using DentalHub.Infrastructure.Repository.PatientRepo;
using DentalHub.Infrastructure.Repository.RequestRepo;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DentalHub.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register DbContext with MySQL
            services.AddDbContext<ContextApp>(options =>
                options.UseMySql(
                    configuration.GetConnectionString("DefaultConnection"),
                    ServerVersion.AutoDetect(configuration.GetConnectionString("DefaultConnection"))
                ));

            // Register Generic Repository
            services.AddScoped(typeof(IMainRepository<>), typeof(MainRepository<>));

            // Register Unit of Work
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>();
            services.AddScoped<ICaseRequestRepository, CaseRequestRepository>();
            services.AddScoped<IPatientCaseRepository, PatientCaseRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();

            return services;
        }
    }
}
