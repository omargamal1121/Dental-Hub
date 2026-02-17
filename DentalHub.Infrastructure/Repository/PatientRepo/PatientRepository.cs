using DentalHub.Application.Interfaces;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.ContextAndConfig;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public async Task< TResult?> GetPatientByPublicId<TResult>(string publicId, ISpecificationWithProjection<Patient, TResult> spec)
		{
		var query=	patients.AsQueryable().AsNoTracking().Where(p=>p.PublicId==publicId).Include(p => p.PatientCases).ThenInclude(pc => pc.CaseType);
		 return	await SpecificationWithProjectionEvaluator<Patient,TResult>.ProjectionOnly(query,spec).FirstOrDefaultAsync();
		}
	}
}
