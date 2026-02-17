using DentalHub.Application.Interfaces;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.ContextAndConfig;
using Microsoft.EntityFrameworkCore;

namespace DentalHub.Infrastructure.Repository.RequestRepo
{
	public class CaseRequestRepository: MainRepository<CaseRequest>,ICaseRequestRepository
	{
		private readonly DbSet<CaseRequest> requests;
		public CaseRequestRepository(ContextApp context ):base(context)
		{
			requests = context.CaseRequests;
			
		}
		public async Task<bool> UpdateRequestStatusAsync(Guid id, RequestStatus requestStatus)
		{
			var rows = await requests
				.Where(cr => cr.Id == id)
				.ExecuteUpdateAsync(set => set
					.SetProperty(cr => cr.Status, requestStatus)
					.SetProperty(cr => cr.UpdateAt, cr => DateTime.UtcNow)
				);

			return rows > 0;
		}
		public async Task<bool> RejectOtherRequestsAsync(Guid caseId, Guid approvedRequestId)
		{
			var rows = await requests
				.Where(cr => cr.PatientCaseId == caseId && cr.Id != approvedRequestId)
				.ExecuteUpdateAsync(set => set
					.SetProperty(cr => cr.Status, RequestStatus.Rejected)
					.SetProperty(cr => cr.UpdateAt, cr => DateTime.UtcNow)
				);

			return rows > 0;
		}
		public async Task<bool> TakenOtherRequestsAsync(Guid caseId, Guid approvedRequestId)
		{
			var rows = await requests
				.Where(cr => cr.PatientCaseId == caseId
							 && cr.Id != approvedRequestId
							 && cr.Status == RequestStatus.Pending)
				.ExecuteUpdateAsync(set => set
					.SetProperty(cr => cr.Status, RequestStatus.Taken)
					.SetProperty(cr => cr.UpdateAt, cr => DateTime.UtcNow)
				);

			return rows > 0;
		}

		public async Task<bool> PendingOtherRequestsAsync(Guid caseId)
		{
			var rows = await requests
				.Where(cr => cr.PatientCaseId == caseId && cr.Status==RequestStatus.Taken)
				.ExecuteUpdateAsync(set => set
					.SetProperty(cr => cr.Status, RequestStatus.Pending)
					.SetProperty(cr => cr.UpdateAt, cr => DateTime.UtcNow)
				);

			return rows > 0;
		}
		public async Task<bool> RejectRequstAsync(Guid id)
		{
			var rows = await requests
			.Where(cr => cr.Id == id &&( cr.Status==RequestStatus.Pending|| cr.Status == RequestStatus.UnderReview))
			.ExecuteUpdateAsync(set => set
				.SetProperty(cr => cr.Status, RequestStatus.Rejected)
				.SetProperty(cr => cr.UpdateAt, cr => DateTime.UtcNow)
			);

			return rows > 0;
		}

		public async Task<bool> RejectAllRequestsForCaseAsync(Guid caseId)
		{
			var rows = await requests
				.Where(cr => cr.PatientCaseId == caseId)
				.ExecuteUpdateAsync(set => set
					.SetProperty(cr => cr.Status, RequestStatus.Rejected)
					.SetProperty(cr => cr.UpdateAt, cr => DateTime.UtcNow)
				);

			return rows > 0;
		}

		public async Task<bool> CancelAllStudentRequestsAsync(Guid studentId)
		{
			var rows = await requests
				.Where(cr => cr.StudentId == studentId && cr.Status == RequestStatus.Pending)
				.ExecuteUpdateAsync(set => set
					.SetProperty(cr => cr.DeleteAt, cr => DateTime.UtcNow)
					.SetProperty(cr => cr.UpdateAt, cr => DateTime.UtcNow)
				);

			return rows > 0;
		}
	}
}
