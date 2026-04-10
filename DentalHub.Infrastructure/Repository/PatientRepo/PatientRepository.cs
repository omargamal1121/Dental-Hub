using DentalHub.Application.Interfaces;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.ContextAndConfig;
using Microsoft.EntityFrameworkCore;

namespace DentalHub.Infrastructure.Repository.PatientRepo
{
	public class PatientRepository : MainRepository<Patient>, IPatientRepository
	{
		private readonly DbSet<Patient> patients;
		public PatientRepository(ContextApp contextApp) : base(contextApp)
		{
			{
				patients = contextApp.Patients;

			}
		}
		public async Task< TResult?> GetPatientById<TResult>(Guid id, ISpecificationWithProjection<Patient, TResult> spec)
		{
		var query=	patients.AsQueryable().AsNoTracking().Where(p=>p.Id==id).Include(p => p.PatientCases);
		 return	await SpecificationWithProjectionEvaluator<Patient,TResult>.ProjectionOnly(query,spec).FirstOrDefaultAsync();
		}
	
	}
}
