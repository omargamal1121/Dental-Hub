using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Repository;

namespace DentalHub.Application.Interfaces
{
	public interface IPatientCaseRepository:IMainRepository<PatientCase>
	{
		public  Task<bool> AssineStudentToCaseAsync(Guid CaseId, Guid StudentId);
		Task<bool> UnassignStudentAsync(Guid caseId);
	}
}
