using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using DentalHub.Application.Factories;
using Microsoft.Extensions.Logging;
using DentalHub.Application.Specification.Comman;
using DentalHub.Application.Interfaces;

namespace DentalHub.Application.Services.Cases
{
    public class PatientCaseService : IPatientCaseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PatientCaseService> _logger;
        private readonly IMediaService _mediaService;

        public PatientCaseService(IUnitOfWork unitOfWork, ILogger<PatientCaseService> logger, IMediaService mediaService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediaService = mediaService;
        }

        #region CRUD Operations

        /// Create a new patient case
        public async Task<Result<PatientCaseDto>> CreateCaseAsync(CreateCaseDto dto)
        {
            try
            {

                var patient = await _unitOfWork.Patients.GetByIdAsync(new BaseSpecification<Patient>(i => i.Id == dto.PatientId));

                if (patient == null)
                {
                    return Result<PatientCaseDto>.Failure("Patient not found");
                }
                var casetype = await _unitOfWork.CaseTypes.GetByIdAsync(new BaseSpecification<CaseType>(i => i.Id == dto.CaseTypeId));

                if(casetype == null)
                {
					return Result<PatientCaseDto>.Failure("Case type not found");
				}

				var patientCase = new PatientCase
                {
                    PatientId = patient.Id,
                    Description =dto.Description??string.Empty,
                    IsPublic = dto.IsPublic,
                    UniversityId = dto.UniversityId,
                    //CaseTypeId = casetype.Id,
                    Status = CaseStatus.Pending
                };

                await _unitOfWork.PatientCases.AddAsync(patientCase);
                await _unitOfWork.SaveChangesAsync();

          
                if (dto.Images != null && dto.Images.Any())
                {
                    foreach (var image in dto.Images)
                    {
                        var uploadResult = await _mediaService.SaveCaseMediaAsync(image, patientCase.Id);
                        if (!uploadResult.IsSuccess)
                        {
                            _logger.LogWarning("Failed to upload image during case creation: {Error}", string.Join(", ", uploadResult.Errors ?? new List<string>()));
                        }
                    }
                }

                _logger.LogInformation("Patient case created: {PublicId} for patient {PatientPublicId}",
                    patientCase.Id, dto.PatientId);

                return await GetCaseByIdAsync(patientCase.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient case");
                return Result<PatientCaseDto>.Failure("Error creating case");
            }
        }

        /// Get case by public ID
        public async Task<Result<PatientCaseDto>> GetCaseByIdAsync(Guid publicId)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                    pc => pc.Id == publicId,
                    pc => new PatientCaseDto
                    {
                        Id = pc.Id,
                        PatientId = pc.Patient.Id,
                        PatientName = pc.Patient.User.FullName,
                        PatientAge = pc.Patient.Age,
						Diagnosisdto = pc.Diagnosiss.Select(d => new Diagnosisdto
						{
							Id = d.Id,
							Notes = d.Notes,
							CaseType = d.CaseType.Name,
							DiagnosisStage = d.Stage.ToString(),
							TeethNumbers = d.TeethNumbers
						}).OrderByDescending(d => d.DiagnosisStage).FirstOrDefault(),
                        Status = pc.Status.ToString(),
                        IsPublic = pc.IsPublic,
                        UniversityId = pc.UniversityId,
                        UniversityName = pc.University != null ? pc.University.Name : null,
                        CreateAt = pc.CreateAt,
                        TotalSessions = pc.Sessions.Count,
                        PendingRequests = pc.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending),
                        ImageUrls = pc.Medias.Select(m => m.MediaUrl).ToList()
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
						Id = pc.Id,
						PatientId = pc.Patient.Id,
						PatientName = pc.Patient.User.FullName,
						PatientAge = pc.Patient.Age,
						Diagnosisdto = pc.Diagnosiss.Select(d => new Diagnosisdto
						{
							Id = d.Id,
							Notes = d.Notes,
							CaseType = d.CaseType.Name,
							DiagnosisStage = d.Stage.ToString(),
							TeethNumbers = d.TeethNumbers
						}).OrderByDescending(d => d.DiagnosisStage).FirstOrDefault(),
						Status = pc.Status.ToString(),
                        IsPublic = pc.IsPublic,
                        UniversityId = pc.UniversityId,
                        UniversityName = pc.University != null ? pc.University.Name : null,
						CreateAt = pc.CreateAt,
						TotalSessions = pc.Sessions.Count,
						PendingRequests = pc.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending),
						ImageUrls = pc.Medias.Select(m => m.MediaUrl).ToList()
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
		public async Task<Result<PagedResult<PatientCaseDto>>> GetAllCasesAsync(
		string? patientName,
		string? caseType,
		string? status,
		int page = 1,
		int pageSize = 10)
		{
			try
			{
				CaseStatus? parsedStatus = null;
				if (!string.IsNullOrWhiteSpace(status))
				{
					if (!Enum.TryParse<CaseStatus>(status, ignoreCase: true, out var s))
						return Result<PagedResult<PatientCaseDto>>.Failure(
							$"Invalid status '{status}'. Valid values: {string.Join(", ", Enum.GetNames<CaseStatus>())}", 400);
					parsedStatus = s;
				}

				var nameFilter = patientName?.Trim().ToLower();
				var caseTypeFilter = caseType?.Trim().ToLower();

				var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
					criteria: pc =>
						(parsedStatus == null || pc.Status == parsedStatus) &&
						(nameFilter == null || pc.Patient.User.FullName.ToLower().StartsWith(nameFilter)) &&
						(caseTypeFilter == null || pc.Diagnosiss.Any(d => d.CaseType.Name.ToLower().StartsWith(caseTypeFilter))),

					projection: pc => new PatientCaseDto
					{
						Id = pc.Id,
						PatientId = pc.Patient.Id,
						PatientName = pc.Patient.User.FullName,
						PatientAge = pc.Patient.Age,
						Diagnosisdto = pc.Diagnosiss.Select(d => new Diagnosisdto
						{
							Id = d.Id,
							Notes = d.Notes,
							CaseType = d.CaseType.Name,
							DiagnosisStage = d.Stage.ToString(),
							TeethNumbers = d.TeethNumbers
						}).OrderByDescending(d => d.DiagnosisStage).FirstOrDefault(),
						Status = pc.Status.ToString(),
                        IsPublic = pc.IsPublic,
                        UniversityId = pc.UniversityId,
                        UniversityName = pc.University != null ? pc.University.Name : null,
						CreateAt = pc.CreateAt,
						TotalSessions = pc.Sessions.Count,
						PendingRequests = pc.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending),
						ImageUrls = pc.Medias.Select(m => m.MediaUrl).ToList()
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
				_logger.LogError(ex, "Error getting all cases (patientName={PatientName}, caseType={CaseType}, status={Status})",
					patientName, caseType, status);
				return Result<PagedResult<PatientCaseDto>>.Failure("Error retrieving cases");
			}
		}

		/// Update case information
		public async Task<Result<PatientCaseDto>> UpdateCaseAsync(UpdateCaseDto dto)
        {
            try
            {
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.Id == dto.Id));

                if (patientCase == null)
                {
                    return Result<PatientCaseDto>.Failure("Case not found");
                }

               
                patientCase.UpdateAt = DateTime.UtcNow;

                _unitOfWork.PatientCases.Update(patientCase);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Case updated: {PublicId}", dto.Id);

                return await GetCaseByIdAsync(patientCase.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating case: {PublicId}", dto.Id);
                return Result<PatientCaseDto>.Failure("Error updating case");
            }
        }

        /// Soft delete case
        public async Task<Result> DeleteCaseByIdAsync(Guid publicId)
        {
            try
            {
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.Id == publicId));

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
              
                if (!Enum.TryParse<CaseStatus>(status, out var caseStatus))
                {
                    return Result<PagedResult<PatientCaseDto>>.Failure("Invalid status");
                }

                var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                    pc => pc.Status == caseStatus,
                    pc => new PatientCaseDto
                    {
                        Id = pc.Id,
                        PatientId = pc.Patient.Id,
                        PatientName = pc.Patient.User.FullName,
                        PatientAge = pc.Patient.Age,
						Diagnosisdto = pc.Diagnosiss.Select(d => new Diagnosisdto
						{
							Id = d.Id,
							Notes = d.Notes,
							CaseType = d.CaseType.Name,
							DiagnosisStage = d.Stage.ToString(),
							TeethNumbers = d.TeethNumbers
						}).OrderByDescending(d => d.DiagnosisStage).FirstOrDefault(),
                        Status = pc.Status.ToString(),
                        IsPublic = pc.IsPublic,
                        UniversityId = pc.UniversityId,
                        UniversityName = pc.University != null ? pc.University.Name : null,
                        CreateAt = pc.CreateAt,
                        TotalSessions = pc.Sessions.Count,
                        PendingRequests = pc.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending),
                        ImageUrls = pc.Medias.Select(m => m.MediaUrl).ToList()
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
                _logger.LogError(ex, "Error getting cases by status: {Status}", status);
                return Result<PagedResult<PatientCaseDto>>.Failure("Error retrieving cases");
            }
        }

        /// Get all cases for a specific patient
        public async Task<Result<PagedResult<PatientCaseDto>>> GetPatientCasesAsync(
			Guid patientPublicId, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                    pc => pc.Patient.Id == patientPublicId,
                    pc => new PatientCaseDto
                    {
                        Id = pc.Id,
                        PatientId = pc.PatientId,
                        PatientName = pc.Patient.User.FullName,
                        PatientAge = pc.Patient.Age,
						Diagnosisdto = pc.Diagnosiss.Select(d => new Diagnosisdto
						{
							Id = d.Id,
							Notes = d.Notes,
							CaseType = d.CaseType.Name,
							DiagnosisStage = d.Stage.ToString(),
							TeethNumbers = d.TeethNumbers
						}).OrderByDescending(d => d.DiagnosisStage).FirstOrDefault(),
                        Status = pc.Status.ToString(),
                        IsPublic = pc.IsPublic,
                        UniversityId = pc.UniversityId,
                        UniversityName = pc.University != null ? pc.University.Name : null,
                        CreateAt = pc.CreateAt,
                        TotalSessions = pc.Sessions.Count,
                        PendingRequests = pc.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending),
                        ImageUrls = pc.Medias.Select(m => m.MediaUrl).ToList()
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
                _logger.LogError(ex, "Error getting patient cases: {PatientId}", patientPublicId);
                return Result<PagedResult<PatientCaseDto>>.Failure("Error retrieving cases");
            }
        }

        #endregion

        #region Status Management

        /// Update case status
        /// Change Case (Pending → InProgress → Completed)
        public async Task<Result<PatientCaseDto>> UpdateCaseStatusAsync(Guid publicId, string newStatus)
        {
            try
            {
                // Parse status
                if (!Enum.TryParse<CaseStatus>(newStatus, out var caseStatus))
                {
                    return Result<PatientCaseDto>.Failure("Invalid status");
                }

                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.Id == publicId));

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

                return await GetCaseByIdAsync(publicId);
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
			Guid publicId,
            string? treatmentType,
            CaseStatus newStatus)
        {
            try
            {
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.Id == publicId));

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

                return await GetCaseByIdAsync(publicId);
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

		public async Task<Result> AssignUniversityAsync(Guid patientCaseId, Guid? universityId, bool isPublic, Guid userId, string role)
		{
			try
			{
				var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.Id == patientCaseId));
				if (patientCase == null) return Result.Failure("Patient case not found", 404);

		
				if (patientCase.Status != CaseStatus.Pending && patientCase.Status != CaseStatus.UnderReview)
				{
					return Result.Failure("Case can only be assigned to a university when it is in Pending or UnderReview status.", 400);
				}

				Guid? targetUniversityId = universityId;

				if (role == "Student")
				{
					var student = await _unitOfWork.Students.GetByIdAsync(new BaseSpecificationWithProjection<Student,Guid>(s => s.User.Id == userId, s => s.UniversityId));
					if (student == Guid.Empty) return Result.Failure("Student profile not found", 404);
					targetUniversityId = student;
				}
				else if (role == "Doctor")
				{
					var doctor = await _unitOfWork.Doctors.GetByIdAsync(new BaseSpecificationWithProjection<Doctor,Guid>(
                        d => d.User.Id == userId,d=>d.UniversityId));
					if (Guid.Empty==doctor) return Result.Failure("Doctor profile not found", 404);
					targetUniversityId = doctor;
				}
				else if (role == "Admin")
				{
					if (targetUniversityId == Guid.Empty || targetUniversityId == null) 
                    {
                        return Result.Failure("Admin must provide a valid UniversityId.", 400);
                    }
                    
				}
			

				patientCase.UniversityId = targetUniversityId;
				patientCase.IsPublic = isPublic;

				_unitOfWork.PatientCases.Update(patientCase);
				await _unitOfWork.SaveChangesAsync();

				return Result.Success();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error assigning university for case");
				return Result.Failure("Error assigning university", 500);
			}
		}
    }
}
