using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.DTOs.CaseTypes;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using DentalHub.Application.Factories;
using Microsoft.Extensions.Logging;
using DentalHub.Application.Specification.Comman;

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

                var patient = await _unitOfWork.Patients.GetByIdAsync(new BaseSpecification<Patient>(i => i.PublicId == dto.PatientId));

                if (patient == null)
                {
                    return Result<PatientCaseDto>.Failure("Patient not found");
                }
                var casetype = await _unitOfWork.CaseTypes.GetByIdAsync(new BaseSpecification<CaseType>(i => i.PublicId == dto.CaseTypeId));

                if(casetype == null)
                {
					return Result<PatientCaseDto>.Failure("Case type not found");
				}

				var patientCase = new PatientCase
                {
                    PatientId = patient.UserId,
                    Description =dto.Description??string.Empty,
                    CaseTypeId = casetype.Id,
                    Status = CaseStatus.Pending
                };

                await _unitOfWork.PatientCases.AddAsync(patientCase);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Patient case created: {PublicId} for patient {PatientPublicId}",
                    patientCase.PublicId, dto.PatientId);

                return await GetCaseByPublicIdAsync(patientCase.PublicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient case");
                return Result<PatientCaseDto>.Failure("Error creating case");
            }
        }

        /// Get case by public ID
        public async Task<Result<PatientCaseDto>> GetCaseByPublicIdAsync(string publicId)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                    pc => pc.PublicId == publicId,
                    pc => new PatientCaseDto
                    {
                        Id = pc.PublicId,
                        PatientId = pc.Patient.PublicId,
                        PatientName = pc.Patient.User.FullName,
                        PatientAge = pc.Patient.Age,
                        CaseType = new CaseTypeDto { Id = pc.CaseType.PublicId, Name = pc.CaseType.Name, Description = pc.CaseType.Description },
                        
                        Status = pc.Status.ToString(),
                        CreateAt = pc.CreateAt,
                        TotalSessions = pc.Sessions.Count,
                        PendingRequests = pc.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending)
                    }
                );
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(spec);

                if (patientCase == null)
                {
                    return Result<PatientCaseDto>.Failure("Case not found");
                }

                return Result<PatientCaseDto>.Success(patientCase);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting case by public ID: {PublicId}", publicId);
                return Result<PatientCaseDto>.Failure("Error retrieving case");
            }
        }

        
        public async Task<Result<PagedResult<PatientCaseDto>>> GetAllCasesAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                    pc => new PatientCaseDto
                    {
                        Id = pc.PublicId,
                        PatientId = pc.Patient.PublicId,
                        PatientName = pc.Patient.User.FullName,
                        PatientAge = pc.Patient.Age,
                        CaseType = new CaseTypeDto { Id = pc.CaseType.PublicId, Name = pc.CaseType.Name, Description = pc.CaseType.Description },
                        Status = pc.Status.ToString(),
                        CreateAt = pc.CreateAt,
                        TotalSessions = pc.Sessions.Count,
                        PendingRequests = pc.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending)
                    }
                );

                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(pc => pc.CreateAt);

				var casesList = await _unitOfWork.PatientCases.GetAllAsync(spec);
				var totalCount = await _unitOfWork.PatientCases.CountAsync(spec);

				var pagedResult = PaginationFactory<PatientCaseDto>.Create(
					count: totalCount,
					page: page,
					pageSize: pageSize,
					data: casesList
				);

				return Result<PagedResult<PatientCaseDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all cases");
                return Result<PagedResult<PatientCaseDto>>.Failure("Error retrieving cases");
            }
        }

        /// Update case information
        public async Task<Result<PatientCaseDto>> UpdateCaseAsync(UpdateCaseDto dto)
        {
            try
            {
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.PublicId == dto.Id));

                if (patientCase == null)
                {
                    return Result<PatientCaseDto>.Failure("Case not found");
                }

               
                patientCase.UpdateAt = DateTime.UtcNow;

                _unitOfWork.PatientCases.Update(patientCase);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Case updated: {PublicId}", dto.Id);

                return await GetCaseByPublicIdAsync(patientCase.PublicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating case: {PublicId}", dto.Id);
                return Result<PatientCaseDto>.Failure("Error updating case");
            }
        }

        /// Soft delete case
        public async Task<Result> DeleteCaseByPublicIdAsync(string publicId)
        {
            try
            {
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.PublicId == publicId));

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

                _logger.LogInformation("Case deleted: {PublicId}", publicId);

                return Result.Success("Case deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting case: {PublicId}", publicId);
                return Result.Failure("Error deleting case");
            }
        }

        #endregion

        #region Query Operations

        /// Get cases by status
        public async Task<Result<PagedResult<PatientCaseDto>>> GetCasesByStatusAsync(
            string status, int page = 1, int pageSize = 10)
        {
            try
            {
                // Parse status
                if (!Enum.TryParse<CaseStatus>(status, out var caseStatus))
                {
                    return Result<PagedResult<PatientCaseDto>>.Failure("Invalid status");
                }

                var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                    pc => pc.Status == caseStatus,
                    pc => new PatientCaseDto
                    {
                        Id = pc.PublicId,
                        PatientId = pc.Patient.PublicId,
                        PatientName = pc.Patient.User.FullName,
                        PatientAge = pc.Patient.Age,
                        CaseType = new CaseTypeDto { Id = pc.CaseType.PublicId, Name = pc.CaseType.Name, Description = pc.CaseType.Description },
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

				var casesList = await _unitOfWork.PatientCases.GetAllAsync(spec);
				var totalCount = await _unitOfWork.PatientCases.CountAsync(spec);

				var pagedResult = PaginationFactory<PatientCaseDto>.Create(
					count: totalCount,
					page: page,
					pageSize: pageSize,
					data: casesList
				);

				return Result<PagedResult<PatientCaseDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cases by status: {Status}", status);
                return Result<PagedResult<PatientCaseDto>>.Failure("Error retrieving cases");
            }
        }

        /// Get all cases for a specific patient
        public async Task<Result<PagedResult<PatientCaseDto>>> GetPatientCasesAsync(
            string patientPublicId, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                    pc => pc.Patient.PublicId == patientPublicId,
                    pc => new PatientCaseDto
                    {
                        Id = pc.PublicId,
                        PatientId = pc.Patient.PublicId,
                        PatientName = pc.Patient.User.FullName,
                        PatientAge = pc.Patient.Age,
                        CaseType = new CaseTypeDto { Id = pc.CaseType.PublicId, Name = pc.CaseType.Name, Description = pc.CaseType.Description },
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

				var casesList = await _unitOfWork.PatientCases.GetAllAsync(spec);
				var totalCount = await _unitOfWork.PatientCases.CountAsync(spec);

				var pagedResult = PaginationFactory<PatientCaseDto>.Create(
					count: totalCount,
					page: page,
					pageSize: pageSize,
					data: casesList
				);

				return Result<PagedResult<PatientCaseDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient cases: {PatientId}", patientPublicId);
                return Result<PagedResult<PatientCaseDto>>.Failure("Error retrieving cases");
            }
        }

        #endregion

        #region Status Management

        /// Update case status
        /// Change Case (Pending → InProgress → Completed)
        public async Task<Result<PatientCaseDto>> UpdateCaseStatusAsync(string publicId, string newStatus)
        {
            try
            {
                // Parse status
                if (!Enum.TryParse<CaseStatus>(newStatus, out var caseStatus))
                {
                    return Result<PatientCaseDto>.Failure("Invalid status");
                }

                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.PublicId == publicId));

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

                _logger.LogInformation("Case status updated: {PublicId} to {Status}", publicId, newStatus);

                return await GetCaseByPublicIdAsync(publicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating case status: {PublicId}", publicId);
                return Result<PatientCaseDto>.Failure("Error updating case status");
            }
        }

        /// ADDED: Combined method to update both treatment type and status in one transaction
        /// This prevents double database calls and ensures data consistency
        public async Task<Result<PatientCaseDto>> UpdateCaseWithStatusAsync(
            string publicId,
            string? treatmentType,
            CaseStatus newStatus)
        {
            try
            {
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.PublicId == publicId));

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

              
                patientCase.Status = newStatus;
                patientCase.UpdateAt = DateTime.UtcNow;

                _unitOfWork.PatientCases.Update(patientCase);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Case updated: {PublicId} - TreatmentType: {TreatmentType}, Status: {Status}",
                    publicId, treatmentType ?? "unchanged", newStatus);

                return await GetCaseByPublicIdAsync(publicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating case with status: {PublicId}", publicId);
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
