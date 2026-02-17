using DentalHub.Application.Interfaces;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.ContextAndConfig;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DentalHub.Infrastructure.Repository.PatientCaseRepo
{
	public class PatientCaseRepository : MainRepository<PatientCase>, IPatientCaseRepository
	{
		private readonly DbSet<PatientCase> _patientCases;
		public PatientCaseRepository(ContextApp contextApp): base(contextApp) 
		{
				_patientCases = contextApp.PatientCases;
				
			
		}
		public async Task<bool> AssineStudentToCaseAsync(Guid CaseId, Guid StudentId)
		{
		 var result=await	_patientCases.Where(pc => pc.Id == CaseId).ExecuteUpdateAsync(
				setter => setter.SetProperty(pc => pc.AssignedStudentId, StudentId).
				SetProperty(pc=>pc.Status, CaseStatus.InProgress).SetProperty(pc => pc.UpdateAt,DateTime.UtcNow)
				
				);

			return result > 0;
		}
		public async Task<bool> UnassignStudentAsync(Guid caseId)
		{
			var result = await _patientCases
				.Where(pc => pc.Id == caseId
						  && (pc.Status == CaseStatus.InProgress|| pc.Status == CaseStatus.UnderReview)
						  )
				.ExecuteUpdateAsync(setter => setter
					.SetProperty(pc => pc.AssignedStudentId, (Guid?)null)
					.SetProperty(pc => pc.Status, CaseStatus.Pending)
					.SetProperty(pc => pc.UpdateAt, DateTime.UtcNow)
				);

			return result > 0;
		}


	}
}
