using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Specification;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Services.Cases
{
    public class PatientCaseService : IPatientCaseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PatientCaseService> _logger;

        public PatientCaseService(IUnitOfWork unitOfWork, ILogger<PatientCaseService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region CRUD Operations

        /// Create a new patient case
        public async Task<Result<PatientCaseDto>> CreateCaseAsync(CreateCaseDto dto)
        {
            try
            {
                // Validate patient exists
                var patient = await _unitOfWork.Patients.GetByIdAsync(
                    new BaseSpecification<Patient>(p => p.UserId == dto.PatientId));

                if (patient == null)
                {
                    return Result<PatientCaseDto>.Failure("Patient not found");
                }

                // Create new case
                var patientCase = new PatientCase
                {
                    Id = Guid.NewGuid(),
                    PatientId = dto.PatientId,
                    TreatmentType = dto.TreatmentType,
                    Status = CaseStatus.Pending,
                    CreateAt = DateTime.UtcNow
                };

                await _unitOfWork.PatientCases.AddAsync(patientCase);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Patient case created: {CaseId} for patient {PatientId}",
                    patientCase.Id, dto.PatientId);

                return await GetCaseByIdAsync(patientCase.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient case");
                return Result<PatientCaseDto>.Failure("Error creating case");
            }
        }

        /// Get case by ID
        public async Task<Result<PatientCaseDto>> GetCaseByIdAsync(Guid caseId)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                    pc => pc.Id == caseId,
                    pc => new PatientCaseDto
                    {
                        Id = pc.Id,
                        PatientId = pc.PatientId,
                        PatientName = pc.Patient.User.FullName,
                        PatientAge = pc.Patient.Age,
                        TreatmentType = pc.TreatmentType,
                        Status = pc.Status.ToString(),
                        CreateAt = pc.CreateAt,
                        TotalSessions = pc.Sessions.Count,
                        PendingRequests = pc.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending)
                    }
                );

                spec.AddInclude("Patient.User");
                spec.AddInclude("Sessions");
                spec.AddInclude("CaseRequests");

                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(spec);

                if (patientCase == null)
                {
                    return Result<PatientCaseDto>.Failure("Case not found");
                }

                return Result<PatientCaseDto>.Success(patientCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting case: {CaseId}", caseId);
                return Result<PatientCaseDto>.Failure("Error retrieving case");
            }
        }

        /// Get all cases with pagination
        public async Task<Result<List<PatientCaseDto>>> GetAllCasesAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                    pc => new PatientCaseDto
                    {
                        Id = pc.Id,
                        PatientId = pc.PatientId,
                        PatientName = pc.Patient.User.FullName,
                        PatientAge = pc.Patient.Age,
                        TreatmentType = pc.TreatmentType,
                        Status = pc.Status.ToString(),
                        CreateAt = pc.CreateAt,
                        TotalSessions = pc.Sessions.Count,
                        PendingRequests = pc.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending)
                    }
                );

                spec.AddInclude("Patient.User");
                spec.AddInclude("Sessions");
                spec.AddInclude("CaseRequests");
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(pc => pc.CreateAt);

                var cases = await _unitOfWork.PatientCases.GetAllAsync(spec);

                return Result<List<PatientCaseDto>>.Success(cases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all cases");
                return Result<List<PatientCaseDto>>.Failure("Error retrieving cases");
            }
        }

        /// Update case information
        public async Task<Result<PatientCaseDto>> UpdateCaseAsync(UpdateCaseDto dto)
        {
            try
            {
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(dto.Id);

                if (patientCase == null)
                {
                    return Result<PatientCaseDto>.Failure("Case not found");
                }

                // Update fields if provided
                if (!string.IsNullOrWhiteSpace(dto.TreatmentType))
                {
                    patientCase.TreatmentType = dto.TreatmentType;
                }

                patientCase.UpdateAt = DateTime.UtcNow;

                _unitOfWork.PatientCases.Update(patientCase);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Case updated: {CaseId}", dto.Id);

                return await GetCaseByIdAsync(dto.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating case: {CaseId}", dto.Id);
                return Result<PatientCaseDto>.Failure("Error updating case");
            }
        }

        /// Soft delete case
        public async Task<Result> DeleteCaseAsync(Guid caseId)
        {
            try
            {
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(caseId);

                if (patientCase == null)
                {
                    return Result.Failure("Case not found");
                }

                // Check if case can be deleted (only Pending or Cancelled)
                if (patientCase.Status == CaseStatus.InProgress)
                {
                    return Result.Failure("Cannot delete a case that is in progress");
                }

                patientCase.DeleteAt = DateTime.UtcNow;
                _unitOfWork.PatientCases.Update(patientCase);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Case deleted: {CaseId}", caseId);

                return Result.Success("Case deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting case: {CaseId}", caseId);
                return Result.Failure("Error deleting case");
            }
        }

        #endregion

        #region Query Operations

        /// Get cases by status
        public async Task<Result<List<PatientCaseDto>>> GetCasesByStatusAsync(
            string status, int page = 1, int pageSize = 10)
        {
            try
            {
                // Parse status
                if (!Enum.TryParse<CaseStatus>(status, out var caseStatus))
                {
                    return Result<List<PatientCaseDto>>.Failure("Invalid status");
                }

                var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                    pc => pc.Status == caseStatus,
                    pc => new PatientCaseDto
                    {
                        Id = pc.Id,
                        PatientId = pc.PatientId,
                        PatientName = pc.Patient.User.FullName,
                        PatientAge = pc.Patient.Age,
                        TreatmentType = pc.TreatmentType,
                        Status = pc.Status.ToString(),
                        CreateAt = pc.CreateAt,
                        TotalSessions = pc.Sessions.Count,
                        PendingRequests = pc.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending)
                    }
                );

                spec.AddInclude("Patient.User");
                spec.AddInclude("Sessions");
                spec.AddInclude("CaseRequests");
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(pc => pc.CreateAt);

                var cases = await _unitOfWork.PatientCases.GetAllAsync(spec);

                return Result<List<PatientCaseDto>>.Success(cases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cases by status: {Status}", status);
                return Result<List<PatientCaseDto>>.Failure("Error retrieving cases");
            }
        }

        /// Get all cases for a specific patient
        public async Task<Result<List<PatientCaseDto>>> GetPatientCasesAsync(
            Guid patientId, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                    pc => pc.PatientId == patientId,
                    pc => new PatientCaseDto
                    {
                        Id = pc.Id,
                        PatientId = pc.PatientId,
                        PatientName = pc.Patient.User.FullName,
                        PatientAge = pc.Patient.Age,
                        TreatmentType = pc.TreatmentType,
                        Status = pc.Status.ToString(),
                        CreateAt = pc.CreateAt,
                        TotalSessions = pc.Sessions.Count,
                        PendingRequests = pc.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending)
                    }
                );

                spec.AddInclude("Patient.User");
                spec.AddInclude("Sessions");
                spec.AddInclude("CaseRequests");
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(pc => pc.CreateAt);

                var cases = await _unitOfWork.PatientCases.GetAllAsync(spec);

                return Result<List<PatientCaseDto>>.Success(cases);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient cases: {PatientId}", patientId);
                return Result<List<PatientCaseDto>>.Failure("Error retrieving cases");
            }
        }

        #endregion

        #region Status Management

        /// Update case status
        /// Change Case (Pending → InProgress → Completed)
        public async Task<Result<PatientCaseDto>> UpdateCaseStatusAsync(Guid caseId, string newStatus)
        {
            try
            {
                // Parse status
                if (!Enum.TryParse<CaseStatus>(newStatus, out var caseStatus))
                {
                    return Result<PatientCaseDto>.Failure("Invalid status");
                }

                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(caseId);

                if (patientCase == null)
                {
                    return Result<PatientCaseDto>.Failure("Case not found");
                }

                // Validate status transition
                if (!IsValidStatusTransition(patientCase.Status, caseStatus))
                {
                    return Result<PatientCaseDto>.Failure($"Cannot change status from {patientCase.Status} to {caseStatus}");
                }

                patientCase.Status = caseStatus;
                patientCase.UpdateAt = DateTime.UtcNow;

                _unitOfWork.PatientCases.Update(patientCase);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Case status updated: {CaseId} to {Status}", caseId, newStatus);

                return await GetCaseByIdAsync(caseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating case status: {CaseId}", caseId);
                return Result<PatientCaseDto>.Failure("Error updating case status");
            }
        }

        /// ADDED: Combined method to update both treatment type and status in one transaction
        /// This prevents double database calls and ensures data consistency
        public async Task<Result<PatientCaseDto>> UpdateCaseWithStatusAsync(
            Guid caseId,
            string? treatmentType,
            CaseStatus newStatus)
        {
            try
            {
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(caseId);

                if (patientCase == null)
                {
                    return Result<PatientCaseDto>.Failure("Case not found");
                }

                // Validate status transition
                if (!IsValidStatusTransition(patientCase.Status, newStatus))
                {
                    return Result<PatientCaseDto>.Failure(
                        $"Cannot change status from {patientCase.Status} to {newStatus}");
                }

                // Update treatment type if provided
                if (!string.IsNullOrWhiteSpace(treatmentType))
                {
                    patientCase.TreatmentType = treatmentType;
                }

                // Update status
                patientCase.Status = newStatus;
                patientCase.UpdateAt = DateTime.UtcNow;

                _unitOfWork.PatientCases.Update(patientCase);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Case updated: {CaseId} - TreatmentType: {TreatmentType}, Status: {Status}",
                    caseId, treatmentType ?? "unchanged", newStatus);

                return await GetCaseByIdAsync(caseId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating case with status: {CaseId}", caseId);
                return Result<PatientCaseDto>.Failure("Error updating case");
            }
        }

        /// Validate if status transition is allowed
        private bool IsValidStatusTransition(CaseStatus currentStatus, CaseStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                // Pending can go to InProgress or Cancelled
                (CaseStatus.Pending, CaseStatus.InProgress) => true,
                (CaseStatus.Pending, CaseStatus.Cancelled) => true,

                // InProgress can go to Completed or Cancelled
                (CaseStatus.InProgress, CaseStatus.Completed) => true,
                (CaseStatus.InProgress, CaseStatus.Cancelled) => true,

                // Completed and Cancelled are final states
                (CaseStatus.Completed, _) => false,
                (CaseStatus.Cancelled, _) => false,

                // Same status is allowed (no change)
                _ when currentStatus == newStatus => true,

                // All other transitions are not allowed
                _ => false
            };
        }

        #endregion
    }
}
