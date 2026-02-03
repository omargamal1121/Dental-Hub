using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Specification;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Services.Cases
{
    /// <summary>
    /// NEW: Complete implementation of CaseRequestService
    /// Handles student requests to doctors for case approval
    /// </summary>
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

        /// <summary>
        /// Create a new case request
        /// الطالب يعمل طلب للدكتور للموافقة على حالة
        /// </summary>
        public async Task<Result<CaseRequestDto>> CreateRequestAsync(CreateCaseRequestDto dto)
        {
            try
            {
                // VALIDATION 1: Check if patient case exists and is available (Pending)
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(dto.PatientCaseId);
                if (patientCase == null)
                {
                    return Result<CaseRequestDto>.Failure("Patient case not found");
                }

                if (patientCase.Status != CaseStatus.Pending)
                {
                    return Result<CaseRequestDto>.Failure(
                        $"Case is not available. Current status: {patientCase.Status}");
                }

                // VALIDATION 2: Check if student exists
                var student = await _unitOfWork.Students.GetByIdAsync(
                    new BaseSpecification<Student>(s => s.UserId == dto.StudentId));
                if (student == null)
                {
                    return Result<CaseRequestDto>.Failure("Student not found");
                }

                // VALIDATION 3: Check if doctor exists
                var doctor = await _unitOfWork.Doctors.GetByIdAsync(
                    new BaseSpecification<Doctor>(d => d.UserId == dto.DoctorId));
                if (doctor == null)
                {
                    return Result<CaseRequestDto>.Failure("Doctor not found");
                }

                // BUSINESS RULE 1: Check if student already has a pending/approved request for this case
                var existingRequestSpec = new BaseSpecification<CaseRequest>(cr =>
                    cr.PatientCaseId == dto.PatientCaseId &&
                    cr.StudentId == dto.StudentId &&
                    (cr.Status == RequestStatus.Pending || cr.Status == RequestStatus.Approved));

                var existingRequest = await _unitOfWork.CaseRequests.GetByIdAsync(existingRequestSpec);
                if (existingRequest != null)
                {
                    return Result<CaseRequestDto>.Failure(
                        $"You already have a {existingRequest.Status} request for this case");
                }

                // BUSINESS RULE 2: Check if case already has an approved request from another student
                var approvedRequestSpec = new BaseSpecification<CaseRequest>(cr =>
                    cr.PatientCaseId == dto.PatientCaseId &&
                    cr.Status == RequestStatus.Approved);

                var approvedRequest = await _unitOfWork.CaseRequests.GetByIdAsync(approvedRequestSpec);
                if (approvedRequest != null)
                {
                    return Result<CaseRequestDto>.Failure(
                        "This case has already been approved for another student");
                }

                // Create the request
                var caseRequest = new CaseRequest
                {
                    Id = Guid.NewGuid(),
                    PatientCaseId = dto.PatientCaseId,
                    StudentId = dto.StudentId,
                    DoctorId = dto.DoctorId,
                    Description = dto.Description,
                    Status = RequestStatus.Pending,
                    CreateAt = DateTime.UtcNow
                };

                await _unitOfWork.CaseRequests.AddAsync(caseRequest);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Case request created: {RequestId} - Student: {StudentId}, Case: {CaseId}, Doctor: {DoctorId}",
                    caseRequest.Id, dto.StudentId, dto.PatientCaseId, dto.DoctorId);

                return await GetRequestByIdAsync(caseRequest.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating case request");
                return Result<CaseRequestDto>.Failure("Error creating request");
            }
        }

        #endregion

        #region Get Requests

        /// <summary>
        /// Get all requests for a specific doctor
        /// الطلبات اللي جايالو الدكتور
        /// </summary>
        public async Task<Result<List<CaseRequestDto>>> GetRequestsByDoctorIdAsync(
            Guid doctorId, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<CaseRequest, CaseRequestDto>(
                    cr => cr.DoctorId == doctorId,
                    cr => new CaseRequestDto
                    {
                        Id = cr.Id,
                        PatientCaseId = cr.PatientCaseId,
                        PatientName = cr.PatientCase.Patient.User.FullName,
                        TreatmentType = cr.PatientCase.TreatmentType,
                        StudentId = cr.StudentId,
                        StudentName = cr.Student.User.FullName,
                        University = cr.Student.University,
                        Level = cr.Student.Level,
                        DoctorId = cr.DoctorId,
                        DoctorName = cr.Doctor.User.FullName,
                        Description = cr.Description,
                        Status = cr.Status.ToString(),
                        CreateAt = cr.CreateAt
                    }
                );

                spec.AddInclude("PatientCase.Patient.User");
                spec.AddInclude("Student.User");
                spec.AddInclude("Doctor.User");
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(cr => cr.CreateAt);

                var requests = await _unitOfWork.CaseRequests.GetAllAsync(spec);

                return Result<List<CaseRequestDto>>.Success(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting requests for doctor: {DoctorId}", doctorId);
                return Result<List<CaseRequestDto>>.Failure("Error retrieving requests");
            }
        }

        /// <summary>
        /// Get all requests made by a specific student
        /// الطلبات اللي الطالب عملها
        /// </summary>
        public async Task<Result<List<CaseRequestDto>>> GetRequestsByStudentIdAsync(
            Guid studentId, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<CaseRequest, CaseRequestDto>(
                    cr => cr.StudentId == studentId,
                    cr => new CaseRequestDto
                    {
                        Id = cr.Id,
                        PatientCaseId = cr.PatientCaseId,
                        PatientName = cr.PatientCase.Patient.User.FullName,
                        TreatmentType = cr.PatientCase.TreatmentType,
                        StudentId = cr.StudentId,
                        StudentName = cr.Student.User.FullName,
                        University = cr.Student.University,
                        Level = cr.Student.Level,
                        DoctorId = cr.DoctorId,
                        DoctorName = cr.Doctor.User.FullName,
                        Description = cr.Description,
                        Status = cr.Status.ToString(),
                        CreateAt = cr.CreateAt
                    }
                );

                spec.AddInclude("PatientCase.Patient.User");
                spec.AddInclude("Student.User");
                spec.AddInclude("Doctor.User");
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(cr => cr.CreateAt);

                var requests = await _unitOfWork.CaseRequests.GetAllAsync(spec);

                return Result<List<CaseRequestDto>>.Success(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting requests for student: {StudentId}", studentId);
                return Result<List<CaseRequestDto>>.Failure("Error retrieving requests");
            }
        }

        /// <summary>
        /// Get request by ID
        /// </summary>
        public async Task<Result<CaseRequestDto>> GetRequestByIdAsync(Guid requestId)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<CaseRequest, CaseRequestDto>(
                    cr => cr.Id == requestId,
                    cr => new CaseRequestDto
                    {
                        Id = cr.Id,
                        PatientCaseId = cr.PatientCaseId,
                        PatientName = cr.PatientCase.Patient.User.FullName,
                        TreatmentType = cr.PatientCase.TreatmentType,
                        StudentId = cr.StudentId,
                        StudentName = cr.Student.User.FullName,
                        University = cr.Student.University,
                        Level = cr.Student.Level,
                        DoctorId = cr.DoctorId,
                        DoctorName = cr.Doctor.User.FullName,
                        Description = cr.Description,
                        Status = cr.Status.ToString(),
                        CreateAt = cr.CreateAt
                    }
                );

                spec.AddInclude("PatientCase.Patient.User");
                spec.AddInclude("Student.User");
                spec.AddInclude("Doctor.User");

                var request = await _unitOfWork.CaseRequests.GetByIdAsync(spec);

                if (request == null)
                {
                    return Result<CaseRequestDto>.Failure("Request not found");
                }

                return Result<CaseRequestDto>.Success(request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting request: {RequestId}", requestId);
                return Result<CaseRequestDto>.Failure("Error retrieving request");
            }
        }

        #endregion

        #region Approve/Reject Request

        /// <summary>
        /// Approve or reject a case request
        /// الدكتور يوافق أو يرفض الطلب
        /// </summary>
        public async Task<Result<CaseRequestDto>> ApproveOrRejectRequestAsync(ApproveCaseRequestDto dto)
        {
            try
            {
                // Get the request with includes
                var spec = new BaseSpecification<CaseRequest>(cr => cr.Id == dto.RequestId);
                spec.AddInclude(cr => cr.PatientCase);

                var request = await _unitOfWork.CaseRequests.GetByIdAsync(spec);

                if (request == null)
                {
                    return Result<CaseRequestDto>.Failure("Request not found");
                }

                // AUTHORIZATION: Verify that this doctor owns the request
                if (request.DoctorId != dto.DoctorId)
                {
                    return Result<CaseRequestDto>.Failure(
                        "You are not authorized to approve/reject this request");
                }

                // VALIDATION: Check if request is still pending
                if (request.Status != RequestStatus.Pending)
                {
                    return Result<CaseRequestDto>.Failure(
                        $"Request has already been {request.Status.ToString().ToLower()}");
                }

                // VALIDATION: Check if case is still available
                if (request.PatientCase.Status != CaseStatus.Pending)
                {
                    return Result<CaseRequestDto>.Failure(
                        $"Case is no longer available. Current status: {request.PatientCase.Status}");
                }

                // Update request status
                if (dto.IsApproved)
                {
                    // APPROVE
                    request.Status = RequestStatus.Approved;

                    // Update patient case status to InProgress
                    request.PatientCase.Status = CaseStatus.InProgress;
                    request.PatientCase.UpdateAt = DateTime.UtcNow;

                    // Automatically reject all other pending requests for this case
                    var otherPendingRequestsSpec = new BaseSpecification<CaseRequest>(cr =>
                        cr.PatientCaseId == request.PatientCaseId &&
                        cr.Id != request.Id &&
                        cr.Status == RequestStatus.Pending);

                    var otherPendingRequests = await _unitOfWork.CaseRequests.GetAllAsync(otherPendingRequestsSpec);
                    foreach (var otherRequest in otherPendingRequests)
                    {
                        otherRequest.Status = RequestStatus.Rejected;
                        otherRequest.UpdateAt = DateTime.UtcNow;
                        _unitOfWork.CaseRequests.Update(otherRequest);
                    }

                    _logger.LogInformation(
                        "Request approved: {RequestId} - Rejected {Count} other pending requests",
                        dto.RequestId, otherPendingRequests.Count);
                }
                else
                {
                    // REJECT
                    request.Status = RequestStatus.Rejected;

                    _logger.LogInformation(
                        "Request rejected: {RequestId} - Reason: {Reason}",
                        dto.RequestId, dto.RejectionReason ?? "No reason provided");
                }

                request.UpdateAt = DateTime.UtcNow;

                _unitOfWork.CaseRequests.Update(request);
                await _unitOfWork.SaveChangesAsync();

                return await GetRequestByIdAsync(dto.RequestId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving/rejecting request: {RequestId}", dto.RequestId);
                return Result<CaseRequestDto>.Failure("Error processing request");
            }
        }

        #endregion

        #region Cancel Request

        /// <summary>
        /// Cancel a pending request
        /// الطالب يلغي الطلب بتاعه
        /// </summary>
        public async Task<Result> CancelRequestAsync(Guid requestId, Guid studentId)
        {
            try
            {
                var request = await _unitOfWork.CaseRequests.GetByIdAsync(requestId);

                if (request == null)
                {
                    return Result.Failure("Request not found");
                }

                // AUTHORIZATION: Verify that this student owns the request
                if (request.StudentId != studentId)
                {
                    return Result.Failure("You are not authorized to cancel this request");
                }

                // Can only cancel pending requests
                if (request.Status != RequestStatus.Pending)
                {
                    return Result.Failure($"Cannot cancel a {request.Status.ToString().ToLower()} request");
                }

                // Soft delete by setting DeleteAt
                request.DeleteAt = DateTime.UtcNow;
                _unitOfWork.CaseRequests.Update(request);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Request cancelled: {RequestId} by student {StudentId}", requestId, studentId);

                return Result.Success("Request cancelled successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling request: {RequestId}", requestId);
                return Result.Failure("Error cancelling request");
            }
        }

        #endregion
    }
}
