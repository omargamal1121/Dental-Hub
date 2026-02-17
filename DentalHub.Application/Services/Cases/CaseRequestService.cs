using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Factories;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Specification;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Logging;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DentalHub.Application.Services.Cases
{
   
    public class CaseRequestService : ICaseRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CaseRequestService> _logger;

        public CaseRequestService(IUnitOfWork unitOfWork, ILogger<CaseRequestService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Create Request

    
        public async Task<Result<CaseRequestDto>> CreateRequestAsync(CreateCaseRequestDto dto)
        {
			try
			{

				// 1️⃣ Get PatientCase Id
				var patientCaseId = await _unitOfWork.PatientCases.GetByIdAsync(
					new BaseSpecificationWithProjection<PatientCase, Guid>(
						pc => pc.PublicId == dto.PatientCasePublicId
						   && pc.Status == CaseStatus.Pending,
						pc => pc.Id));

				if (patientCaseId == Guid.Empty)
					return Result<CaseRequestDto>. Failure("Patient case not found or taken");


				// 2️⃣ Get Doctor Id
				var doctorId = await _unitOfWork.Doctors.GetByIdAsync(
					new BaseSpecificationWithProjection<Doctor, Guid>(
						d => d.PublicId == dto.DoctorPublicId,
						d => d.Id));

				if (doctorId == Guid.Empty)
					return Result<CaseRequestDto>.Failure("Doctor not found");


				// 3️⃣ Get Student Id
				var studentId = await _unitOfWork.Students.GetByIdAsync(
					new BaseSpecificationWithProjection<Student, Guid>(
						s => s.PublicId == dto.StudentPublicId,
						s => s.Id));

				if (studentId == Guid.Empty)
					return Result<CaseRequestDto>.Failure("Student not found");


				// 4️⃣ Check duplicate
				var duplicate = await _unitOfWork.CaseRequests.AnyAsync(
					new BaseSpecification<CaseRequest>(cr =>
						cr.PatientCaseId == patientCaseId &&
						cr.StudentId == studentId &&
						cr.DoctorId == doctorId));

				if (duplicate)
					return Result<CaseRequestDto>.Failure("You already have a request for this case");


				// 5️⃣ Create
				var caseRequest = new CaseRequest
				{
					PatientCaseId = patientCaseId,
					StudentId = studentId,
					DoctorId = doctorId,
					Description = dto.Description,
					Status = RequestStatus.Pending
				};


				await _unitOfWork.CaseRequests.AddAsync(caseRequest);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Case request created: {RequestId} - Student: {StudentId}, Case: {CaseId}, Doctor: {DoctorId}",
                    caseRequest.Id, dto.StudentPublicId, dto.PatientCasePublicId, dto.DoctorPublicId);

                return await GetRequestByPublicIdAsync(caseRequest.PublicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating case request");
                return Result<CaseRequestDto>.Failure("Error creating request");
            }
        }

        #endregion

        #region Get Requests


        public async Task<Result<PagedResult<CaseRequestDto>>> GetRequestsByDoctorIdAsync(
            string doctorPublicId, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<CaseRequest, CaseRequestDto>(
                    cr => cr.Doctor.PublicId == doctorPublicId,
                    cr => new CaseRequestDto
                    {
                        Id = cr.PublicId,
                        PatientCaseId = cr.PatientCase.PublicId,
                        PatientName = cr.PatientCase.Patient.User.FullName,
                   
                        StudentId = cr.Student.PublicId,
                        StudentName = cr.Student.User.FullName,
                        University = cr.Student.University,
                        Level = cr.Student.Level,
                        DoctorId = cr.Doctor.PublicId,
                        DoctorName = cr.Doctor.User.FullName,
                        Description = cr.Description,
                        Status = cr.Status.ToString(),
                        CreateAt = cr.CreateAt
                    }
                );

              
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(cr => cr.CreateAt);

				var requestsList = await _unitOfWork.CaseRequests.GetAllAsync(spec);
				var totalCount = await _unitOfWork.CaseRequests.CountAsync(spec);

				var pagedResult = PaginationFactory<CaseRequestDto>.Create(
					count: totalCount,
					page: page,
					pageSize: pageSize,
					data: requestsList
				);

				return Result<PagedResult<CaseRequestDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting requests for doctor: {DoctorId}", doctorPublicId);
                return Result<PagedResult<CaseRequestDto>>.Failure("Error retrieving requests");
            }
        }

        public async Task<Result<PagedResult<CaseRequestDto>>> GetRequestsByStudentIdAsync(
            string studentPublicId, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<CaseRequest, CaseRequestDto>(
                    cr => cr.Student.PublicId == studentPublicId,
                    cr => new CaseRequestDto
                    {
                        Id = cr.PublicId,
                        PatientCaseId = cr.PatientCase.PublicId,
                        PatientName = cr.PatientCase.Patient.User.FullName,
                        CaseName=cr.PatientCase.CaseType.Name,
                        StudentId = cr.Student.PublicId,
                        StudentName = cr.Student.User.FullName,
                        University = cr.Student.University,
                        Level = cr.Student.Level,
                        DoctorId = cr.Doctor.PublicId,
                        DoctorName = cr.Doctor.User.FullName,
                        Description = cr.Description,
                        Status = cr.Status.ToString(),
                        CreateAt = cr.CreateAt
                    }
                );

                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(cr => cr.CreateAt);

				var requestsList = await _unitOfWork.CaseRequests.GetAllAsync(spec);
				var totalCount = await _unitOfWork.CaseRequests.CountAsync(spec);

				var pagedResult = PaginationFactory<CaseRequestDto>.Create(
					count: totalCount,
					page: page,
					pageSize: pageSize,
					data: requestsList
				);

				return Result<PagedResult<CaseRequestDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting requests for student: {StudentId}", studentPublicId);
                return Result<PagedResult<CaseRequestDto>>.Failure("Error retrieving requests");
            }
        }

        public async Task<Result<CaseRequestDto>> GetRequestByPublicIdAsync(string publicId)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<CaseRequest, CaseRequestDto>(
                    cr => cr.PublicId == publicId,
                    cr => new CaseRequestDto
                    {
                        Id = cr.PublicId,
                        PatientCaseId = cr.PatientCase.PublicId,
                        PatientName = cr.PatientCase.Patient.User.FullName,
                        CaseName=  cr.PatientCase.CaseType.Name,
                        StudentId = cr.Student.PublicId,
                        StudentName = cr.Student.User.FullName,
                        University = cr.Student.University,
                        Level = cr.Student.Level,
                        DoctorId = cr.Doctor.PublicId,
                        DoctorName = cr.Doctor.User.FullName,
                        Description = cr.Description,
                        Status = cr.Status.ToString(),
                        CreateAt = cr.CreateAt
                    }
                );


                var request = await _unitOfWork.CaseRequests.GetByIdAsync(spec);

                if (request == null)
                {
                    return Result<CaseRequestDto>.Failure("Request not found");
                }

                return Result<CaseRequestDto>.Success(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting request by public ID: {PublicId}", publicId);
                return Result<CaseRequestDto>.Failure("Error retrieving request");
            }
        }

		#endregion

		public async Task<Result<bool>> ApproveRequstAsync(ApproveCaseRequestDto dto)
		{
			_logger.LogInformation("Starting approval process for RequestId: {RequestId} by Doctor: {DoctorId}",
				dto.RequestId, dto.DoctorId);

			var spec = new BaseSpecificationWithProjection<CaseRequest, CheckOnRequsetAndCase>(
				cr => cr.PublicId == dto.RequestId,
				cr => new CheckOnRequsetAndCase
				{
					IsPending = cr.PatientCase.Status == CaseStatus.UnderReview|| cr.PatientCase.Status == CaseStatus.Pending,
					DoctorPublicId = cr.Doctor.PublicId,
					CaseId = cr.PatientCaseId,
					StudentId = cr.StudentId,
                    RequestId = cr.Id
				});

			var result = await _unitOfWork.CaseRequests.GetByIdAsync(spec);

			if (result == null)
			{
				_logger.LogWarning("Approval failed: Request not found. RequestId: {RequestId}", dto.RequestId);
				return Result<bool>.Failure($"No Request With this Id: {dto.RequestId}", 404);
			}

	
			if (!result.IsPending)
			{
				_logger.LogWarning("Approval rejected: Case already approved. CaseId: {CaseId}", result.CaseId);
				return Result<bool>.Failure("Case already approved by someone else", 409);
			}

			if (result.DoctorPublicId != dto.DoctorId)
			{
				_logger.LogWarning("Unauthorized approval attempt. RequestId: {RequestId}, DoctorId: {DoctorId}",
					dto.RequestId, dto.DoctorId);
				return Result<bool>.Failure("No Rights", 401);
			}

			await _unitOfWork.BeginTransactionAsync();

			try
			{
				var isUpdated = await _unitOfWork.CaseRequests
					.UpdateRequestStatusAsync(result.RequestId, RequestStatus.Approved);

				if (!isUpdated)
					throw new Exception("Failed to update request status.");

				_logger.LogInformation("Request approved. RequestId: {RequestId}", dto.RequestId);

				var isAssigned = await _unitOfWork.PatientCases
					.AssineStudentToCaseAsync(result.CaseId, result.StudentId);

				if (!isAssigned)
					throw new Exception("Failed to assign student to case.");

				_logger.LogInformation("Student assigned to case. CaseId: {CaseId}, StudentId: {StudentId}",
					result.CaseId, result.StudentId);

			
				await _unitOfWork.CaseRequests
					.TakenOtherRequestsAsync(result.CaseId, result.RequestId);

				_logger.LogInformation("Other requests updated for CaseId: {CaseId}", result.CaseId);

				await _unitOfWork.CommitTransactionAsync();

				_logger.LogInformation("Approval transaction committed successfully. RequestId: {RequestId}",
					dto.RequestId);

				return Result<bool>.Success(true, "Updated");
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackTransactionAsync();

				_logger.LogError(ex,
					"Error occurred while approving request. RequestId: {RequestId}", dto.RequestId);

				return Result<bool>.Failure("An error occurred while approving the request", 500);
			}
		}




		public async Task<Result<bool>> RejectRequestAsync(string publicId, string doctorPublicId)
		{
			_logger.LogInformation("Starting reject process for RequestId: {PublicId}", publicId);

			var spec = new BaseSpecificationWithProjection<CaseRequest, RejectCheckDto>(
				cr => cr.PublicId == publicId,
				cr => new RejectCheckDto
				{
					status = cr.Status,
					DoctorPublicId = cr.Doctor.PublicId,
					CaseId = cr.PatientCaseId,
					StudentId = cr.StudentId,
                    RequestId = cr.Id
				});

			var request = await _unitOfWork.CaseRequests.GetByIdAsync(spec);

			if (request == null)
			{
				_logger.LogWarning("Request not found: {RequestId}", publicId);
				return Result<bool>.Failure("Request not found", 404);
			}

			if (request.DoctorPublicId != doctorPublicId)
			{
				_logger.LogWarning("Doctor {DoctorPublicId} has no rights to reject request {PublicId}", doctorPublicId, publicId);
				return Result<bool>.Failure("No rights", 401);
			}

			await _unitOfWork.BeginTransactionAsync();

			try
			{
				if (request.status == RequestStatus.Pending)
				{
					_logger.LogInformation("Rejecting pending request {PublicId}", publicId);
					var updated = await _unitOfWork.CaseRequests.RejectRequstAsync(request.RequestId);
					if (!updated)
						throw new Exception("Failed to update request status");
				}
				else if (request.status == RequestStatus.UnderReview)
				{
					_logger.LogInformation("Reverting approved/under-review request {RequestId}", publicId);

			
					var unassigned = await _unitOfWork.PatientCases.UnassignStudentAsync(request.CaseId);
					if (!unassigned)
						throw new Exception("Failed to unassign student from case");

					
					var pendingUpdated = await _unitOfWork.CaseRequests.PendingOtherRequestsAsync(request.CaseId);
					if (!pendingUpdated)
						_logger.LogInformation("No other requests needed to be reverted to Pending for CaseId: {CaseId}", request.CaseId);

					var rejected = await _unitOfWork.CaseRequests.RejectRequstAsync(request.RequestId);
					if (!rejected)
						throw new Exception("Failed to reject the request");
				}
                else
                {
                    return Result<bool>.Failure($"Can't Update request from:{request.status} To: Reject");
                }

                    await _unitOfWork.CommitTransactionAsync();

				_logger.LogInformation("Request successfully reverted: {RequestId}", publicId);
				return Result<bool>.Success(true, "Request reverted successfully");
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackTransactionAsync();
				_logger.LogError(ex, "Error while reverting request {RequestId}", publicId);
				return Result<bool>.Failure("Error while reverting request", 500);
			}
		}


		#region Cancel Request


		public async Task<Result> CancelRequestAsync(string publicId, string studentPublicId)
        {
            try
            {
                var request = await _unitOfWork.CaseRequests.GetByIdAsync(
                    new BaseSpecification<CaseRequest>(cr => cr.PublicId == publicId));

                if (request == null)
                {
                    return Result.Failure("Request not found");
                }

          
                if (request.Student.PublicId != studentPublicId)
                {
                    return Result.Failure("You are not authorized to cancel this request");
                }

        
                if (request.Status != RequestStatus.Pending)
                {
                    return Result.Failure($"Cannot cancel a {request.Status.ToString().ToLower()} request");
                }

           
                request.DeleteAt = DateTime.UtcNow;
                _unitOfWork.CaseRequests.Update(request);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Request cancelled: {PublicId} by student {StudentPublicId}", publicId, studentPublicId);

                return Result.Success("Request cancelled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling request: {PublicId}", publicId);
                return Result.Failure("Error cancelling request");
            }
        }

        #endregion

		public async Task<Result<bool>> RejectAllRequestsForCaseAsync(string casePublicId)
		{
			try
			{
                var caseEntity = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.PublicId == casePublicId));
                if (caseEntity == null) return Result<bool>.Failure("Case not found");

				_logger.LogInformation("Rejecting all requests for CaseId: {CasePublicId}", casePublicId);
				var result = await _unitOfWork.CaseRequests.RejectAllRequestsForCaseAsync(caseEntity.Id);
				await _unitOfWork.SaveChangesAsync();
				return Result<bool>.Success(result, "All requests rejected");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error rejecting all requests for CaseId: {CasePublicId}", casePublicId);
				return Result<bool>.Failure("Error rejecting requests");
			}
		}

		public async Task<Result<bool>> MarkAllRequestsTakenForCaseAsync(string casePublicId, string approvedRequestPublicId)
		{
			try
			{
                var caseEntity = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.PublicId == casePublicId));
                if (caseEntity == null) return Result<bool>.Failure("Case not found");

                var requestEntity = await _unitOfWork.CaseRequests.GetByIdAsync(new BaseSpecification<CaseRequest>(cr => cr.PublicId == approvedRequestPublicId));
                if (requestEntity == null) return Result<bool>.Failure("Request not found");

				_logger.LogInformation("Marking all other requests as Taken for Case: {CasePublicId}, excluding Request: {RequestPublicId}", casePublicId, approvedRequestPublicId);
				var result = await _unitOfWork.CaseRequests.TakenOtherRequestsAsync(caseEntity.Id, requestEntity.Id);
				await _unitOfWork.SaveChangesAsync();
				return Result<bool>.Success(result, "Other requests marked as taken");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error marking requests as taken for Case: {CasePublicId}", casePublicId);
				return Result<bool>.Failure("Error marking requests as taken");
			}
		}

		public async Task<Result<IEnumerable<CaseRequestDto>>> GetRequestsByCaseIdAsync(string casePublicId, RequestStatus? status = null)
		{
			try
			{
				_logger.LogInformation("Getting requests for Case: {CasePublicId} with Status: {Status}", casePublicId, status);
				var spec = new BaseSpecificationWithProjection<CaseRequest, CaseRequestDto>(
					cr => cr.PatientCase.PublicId == casePublicId && (!status.HasValue || cr.Status == status.Value),
					cr => new CaseRequestDto
					{
						Id = cr.PublicId,
						PatientCaseId = cr.PatientCase.PublicId,
						PatientName = cr.PatientCase.Patient.User.FullName,
						CaseName = cr.PatientCase.CaseType.Name,
						StudentId = cr.Student.PublicId,
						StudentName = cr.Student.User.FullName,
						University = cr.Student.University,
						Level = cr.Student.Level,
						DoctorId = cr.Doctor.PublicId,
						DoctorName = cr.Doctor.User.FullName,
						Description = cr.Description,
						Status = cr.Status.ToString(),
						CreateAt = cr.CreateAt
					}
				);

				var requestsList = await _unitOfWork.CaseRequests.GetAllAsync(spec);
				return Result<IEnumerable<CaseRequestDto>>.Success(requestsList);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting requests for CaseId: {CaseId}", casePublicId);
				return Result<IEnumerable<CaseRequestDto>>.Failure("Error retrieving requests");
			}
		}

		public async Task<Result<bool>> CancelAllStudentRequestsAsync(string studentPublicId)
		{
			try
			{
                var student = await _unitOfWork.Students.GetByIdAsync(new BaseSpecification<Student>(s => s.PublicId == studentPublicId));
                if (student == null) return Result<bool>.Failure("Student not found");

				_logger.LogInformation("Cancelling all pending requests for Student: {StudentPublicId}", studentPublicId);
				var result = await _unitOfWork.CaseRequests.CancelAllStudentRequestsAsync(student.UserId);
				await _unitOfWork.SaveChangesAsync();
				return Result<bool>.Success(result, "All pending requests cancelled");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error cancelling all requests for Student: {StudentPublicId}", studentPublicId);
				return Result<bool>.Failure("Error cancelling requests");
			}
		}
    }

	public class GetIds
	{
		public Guid Docid { get; set; }
		public Guid Studentid { get; set; }
	}
    public class CheckOnRequsetAndCase 
    {
        public bool IsExsist { get; set; }
        public bool IsPending { get; set; }
        public string DoctorPublicId { get; set; }
		public Guid CaseId { get; set; }
		public Guid StudentId { get; set; }
        public Guid RequestId { get; set; }
	}

    public class RejectCheckDto
    {
        public RequestStatus status { get; set; }
        public string DoctorPublicId { get; set; }
        public Guid CaseId { get; set; }
        public Guid StudentId { get; set; }
        public Guid RequestId { get; set; }
    }
}
