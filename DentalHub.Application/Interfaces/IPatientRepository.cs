using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Repository;

namespace DentalHub.Application.Interfaces
{
	public interface IPatientRepository:IMainRepository<Patient>
	{
		public Task<TResult?> GetPatientByPublicId<TResult>(string publicId, ISpecificationWithProjection<Patient, TResult> spec);


	}
}
