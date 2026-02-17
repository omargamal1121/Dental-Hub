using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Repository;

namespace DentalHub.Application.Interfaces
{
	public interface ICaseRequestRepository:IMainRepository<CaseRequest>
	{
		public Task<bool> UpdateRequestStatusAsync(Guid id, RequestStatus requestStatus);
		public Task<bool> RejectOtherRequestsAsync(Guid caseId, Guid approvedRequestId);
		public  Task<bool> TakenOtherRequestsAsync(Guid caseId, Guid approvedRequestId);
		public  Task<bool> RejectRequstAsync(Guid id);
		public Task<bool> PendingOtherRequestsAsync(Guid caseId);
		public Task<bool> RejectAllRequestsForCaseAsync(Guid caseId);
		public Task<bool> CancelAllStudentRequestsAsync(Guid studentId);
	}
}
