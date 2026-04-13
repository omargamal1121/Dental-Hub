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
                    return Result<PatientCaseDto>.Failure("Patient not found");

                var casetype = await _unitOfWork.CaseTypes.GetByIdAsync(new BaseSpecification<CaseType>(i => i.Id == dto.CaseTypeId));
                if (casetype == null)
                    return Result<PatientCaseDto>.Failure("Case type not found");

                var patientCase = new PatientCase
                {
                    PatientId = patient.Id,
                    Description = dto.Description ?? string.Empty,
                    IsPublic = dto.IsPublic,
                    UniversityId = dto.UniversityId,
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
                            _logger.LogWarning("Failed to upload image during case creation: {Error}", string.Join(", ", uploadResult.Errors ?? new List<string>()));
                    }
                }

                _logger.LogInformation("Patient case created: {PublicId} for patient {PatientPublicId}", patientCase.Id, dto.PatientId);
                return await GetCaseByIdAsync(patientCase.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient case");
                return Result<PatientCaseDto>.Failure("Error creating case");
            }
        }

        /// <summary>
        /// Get case by ID with user-specific flags, process status, and available actions.
        /// </summary>
        public async Task<Result<PatientCaseDto>> GetCaseByIdAsync(Guid publicId, Guid? userId = null, string? userRole = null)
        {
            try
            {
                // ── 1. Fetch raw data we need for flags + process-status computation ──
                var rawSpec = new BaseSpecification<PatientCase>(pc => pc.Id == publicId);
                rawSpec.AddInclude(pc => pc.Sessions); // needed by ComputeProcessStatus (Evaluated check)
                var rawCase = await _unitOfWork.PatientCases.GetByIdAsync(rawSpec);

                if (rawCase == null)
                    return Result<PatientCaseDto>.Failure("Case not found", 404);

                // ── 2. Build the main projection ──
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

                var dto = await _unitOfWork.PatientCases.GetByIdAsync(spec);
                if (dto == null)
                    return Result<PatientCaseDto>.Failure("Case not found", 404);

                // ── 3. Compute ProcessStatus ──
                dto.ProcessStatus = ComputeProcessStatus(rawCase, dto.Diagnosisdto);

                // ── 4. Compute UserFlags ──
                if (userId.HasValue && !string.IsNullOrEmpty(userRole))
                {
                    dto.UserFlags = await ComputeUserFlagsAsync(rawCase, userId.Value, userRole);
                }

                // ── 5. Compute AvailableActions ──
                dto.AvailableActions = ComputeAvailableActions(dto.UserFlags, rawCase);

                return Result<PatientCaseDto>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting case by public ID: {PublicId}", publicId);
                return Result<PatientCaseDto>.Failure("Error retrieving case");
            }
        }

        /// <summary>
        /// Compute a human-readable process status based on case state and diagnosis stage.
        /// Priority logic:
        ///   Completed  → "Completed"
        ///   InProgress → "InProgress"
        ///   Has diagnosis with AdvancedClinic/BasicClinic → "DiagnosedInClinic"
        ///   Has diagnosis with AI stage → "AIPreliminaryDiagnosis"
        ///   No student assigned → "UnAssigned"
        ///   Has completed sessions → "Evaluated"
        ///   Otherwise → maps to Status string
        /// </summary>
        private static string ComputeProcessStatus(PatientCase rawCase, Diagnosisdto? diagnosisDto)
        {
            if (rawCase.Status == CaseStatus.Completed)
                return "Completed";

            if (rawCase.Status == CaseStatus.InProgress)
            {
                // Check if evaluated (has at least one Done session)
                bool hasEvaluatedSession = rawCase.Sessions != null &&
                    rawCase.Sessions.Any(s => s.Status == SessionStatus.Done);

                return hasEvaluatedSession ? "Evaluated" : "InProgress";
            }

            // Pending or UnderReview:
            // Priority: UnAssigned first (no student), then diagnosis stage, then raw status
            if (rawCase.AssignedStudentId == null)
                return "UnAssigned";

            if (diagnosisDto != null)
            {
                if (diagnosisDto.DiagnosisStage == DiagnosisStage.BasicClinic.ToString() ||
                    diagnosisDto.DiagnosisStage == DiagnosisStage.AdvancedClinic.ToString())
                    return "DiagnosedInClinic";

                if (diagnosisDto.DiagnosisStage == DiagnosisStage.AI.ToString())
                    return "AIPreliminaryDiagnosis";
            }

            return rawCase.Status.ToString();
        }

        /// <summary>
        /// Resolve who the current user is relative to this case and return flags.
        /// </summary>
        private async Task<CaseUserFlags> ComputeUserFlagsAsync(PatientCase rawCase, Guid userId, string userRole)
        {
            var flags = new CaseUserFlags
            {
                IsDoctor = userRole == "Doctor",
                IsStudent = userRole == "Student"
            };

            switch (userRole)
            {
                case "Patient":
                    {
                        // Check if this user is the patient who owns the case
                        var patientSpec = new BaseSpecificationWithProjection<Patient, Guid>(
                            p => p.User.Id == userId, p => p.Id);
                        var patientId = await _unitOfWork.Patients.GetByIdAsync(patientSpec);
                        flags.IsOwner = (patientId != Guid.Empty && patientId == rawCase.PatientId);
                        break;
                    }

                case "Doctor":
                    {
                        var doctorSpec = new BaseSpecificationWithProjection<Doctor, Guid>(
                            d => d.User.Id == userId, d => d.Id);
                        var doctorId = await _unitOfWork.Doctors.GetByIdAsync(doctorSpec);

                        flags.IsAssignedDoctor = (doctorId != Guid.Empty && rawCase.AssignedDoctorId == doctorId);

                        // Also check if the doctor has any request on this case
                        var requestSpec = new BaseSpecification<CaseRequest>(
                            cr => cr.PatientCaseId == rawCase.Id && cr.Doctor.User.Id == userId &&
                                  (cr.Status == RequestStatus.Pending || cr.Status == RequestStatus.Approved));
                        var request = await _unitOfWork.CaseRequests.GetByIdAsync(requestSpec);
                        flags.HasPendingRequest = request != null;
                        break;
                    }

                case "Student":
                    {
                        var studentSpec = new BaseSpecificationWithProjection<Student, Guid>(
                            s => s.User.Id == userId, s => s.Id);
                        var studentId = await _unitOfWork.Students.GetByIdAsync(studentSpec);

                        flags.IsAssignedStudent = (studentId != Guid.Empty && rawCase.AssignedStudentId == studentId);

                        // Check if student has a pending/approved request
                        var requestSpec = new BaseSpecification<CaseRequest>(
                            cr => cr.PatientCaseId == rawCase.Id && cr.Student.User.Id == userId &&
                                  (cr.Status == RequestStatus.Pending || cr.Status == RequestStatus.Approved));
                        var request = await _unitOfWork.CaseRequests.GetByIdAsync(requestSpec);
                        flags.HasPendingRequest = request != null;
                        break;
                    }
            }

            return flags;
        }

        /// <summary>
        /// Determine what actions the current user can perform on the case.
        /// </summary>
        private static List<string> ComputeAvailableActions(CaseUserFlags flags, PatientCase rawCase)
        {
            var actions = new List<string>();

            // ── Patient (Owner) ──
            if (flags.IsOwner)
            {
                if (rawCase.Status == CaseStatus.Pending)
                    actions.Add("EditCase");

                actions.Add("ViewSessions");
                actions.Add("ViewDiagnosis");
            }

            // ── Doctor ──
            if (flags.IsDoctor)
            {
                actions.Add("ViewCase");

                if (flags.IsAssignedDoctor)
                {
                    actions.Add("ViewSessions");
                    actions.Add("EvaluateSession");
                    actions.Add("AddDiagnosis");

                    if (rawCase.Status == CaseStatus.InProgress)
                        actions.Add("CompleteCase");
                }
                else if (!flags.HasPendingRequest &&
                         (rawCase.Status == CaseStatus.Pending || rawCase.Status == CaseStatus.UnderReview) &&
                         rawCase.IsPublic)
                {
                    actions.Add("RequestToSupervise");
                }
            }

            // ── Student ──
            if (flags.IsStudent)
            {
                actions.Add("ViewCase");

                if (flags.IsAssignedStudent)
                {
                    actions.Add("ViewSessions");
                    actions.Add("CreateSession");
                    actions.Add("AddSessionNote");
                }
                else if (!flags.HasPendingRequest &&
                         (rawCase.Status == CaseStatus.Pending || rawCase.Status == CaseStatus.UnderReview) &&
                         rawCase.IsPublic)
                {
                    actions.Add("RequestCase");
                }
                else if (flags.HasPendingRequest)
                {
                    actions.Add("CancelRequest");
                }
            }

            return actions;
        }

        #endregion

        #region List Queries (unchanged)

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

                var countSpec = new BaseSpecification<PatientCase>();
                var casesList = await _unitOfWork.PatientCases.GetAllAsync(spec);
                var totalCount = await _unitOfWork.PatientCases.CountAsync(countSpec);

                var pagedResult = PaginationFactory<PatientCaseDto>.Create(
                    count: totalCount, page: page, pageSize: pageSize, data: casesList);

                return Result<PagedResult<PatientCaseDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all cases");
                return Result<PagedResult<PatientCaseDto>>.Failure("Error retrieving cases");
            }
        }

        public async Task<Result<PagedResult<PatientCaseDto>>> GetAllCasesAsync(
            string? patientName, string? caseType, string? status, int page = 1, int pageSize = 10)
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

                // Separate count spec without paging to get correct total
                var countSpec = new BaseSpecification<PatientCase>(
                    pc =>
                        (parsedStatus == null || pc.Status == parsedStatus) &&
                        (nameFilter == null || pc.Patient.User.FullName.ToLower().StartsWith(nameFilter)) &&
                        (caseTypeFilter == null || pc.Diagnosiss.Any(d => d.CaseType.Name.ToLower().StartsWith(caseTypeFilter))));

                var casesList = await _unitOfWork.PatientCases.GetAllAsync(spec);
                var totalCount = await _unitOfWork.PatientCases.CountAsync(countSpec);

                var pagedResult = PaginationFactory<PatientCaseDto>.Create(
                    count: totalCount, page: page, pageSize: pageSize, data: casesList);

                return Result<PagedResult<PatientCaseDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all cases (patientName={PatientName}, caseType={CaseType}, status={Status})",
                    patientName, caseType, status);
                return Result<PagedResult<PatientCaseDto>>.Failure("Error retrieving cases");
            }
        }

        public async Task<Result<PatientCaseDto>> UpdateCaseAsync(UpdateCaseDto dto)
        {
            try
            {
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.Id == dto.Id));
                if (patientCase == null)
                    return Result<PatientCaseDto>.Failure("Case not found");

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

        public async Task<Result> DeleteCaseByIdAsync(Guid publicId)
        {
            try
            {
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.Id == publicId));
                if (patientCase == null)
                    return Result.Failure("Case not found");

                if (patientCase.Status == CaseStatus.InProgress)
                    return Result.Failure("Cannot delete a case that is in progress");

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

        public async Task<Result<PagedResult<PatientCaseDto>>> GetCasesByStatusAsync(string status, int page = 1, int pageSize = 10)
        {
            try
            {
                if (!Enum.TryParse<CaseStatus>(status, out var caseStatus))
                    return Result<PagedResult<PatientCaseDto>>.Failure("Invalid status");

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

                var countSpecStatus = new BaseSpecification<PatientCase>(pc => pc.Status == caseStatus);
                var casesList = await _unitOfWork.PatientCases.GetAllAsync(spec);
                var totalCount = await _unitOfWork.PatientCases.CountAsync(countSpecStatus);

                var pagedResult = PaginationFactory<PatientCaseDto>.Create(
                    count: totalCount, page: page, pageSize: pageSize, data: casesList);

                return Result<PagedResult<PatientCaseDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cases by status: {Status}", status);
                return Result<PagedResult<PatientCaseDto>>.Failure("Error retrieving cases");
            }
        }

        public async Task<Result<PagedResult<PatientCaseDto>>> GetPatientCasesAsync(Guid patientPublicId, int page = 1, int pageSize = 10)
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

                var countSpecPatient = new BaseSpecification<PatientCase>(pc => pc.Patient.Id == patientPublicId);
                var casesList = await _unitOfWork.PatientCases.GetAllAsync(spec);
                var totalCount = await _unitOfWork.PatientCases.CountAsync(countSpecPatient);

                var pagedResult = PaginationFactory<PatientCaseDto>.Create(
                    count: totalCount, page: page, pageSize: pageSize, data: casesList);

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

        public async Task<Result<PatientCaseDto>> UpdateCaseStatusAsync(Guid publicId, string newStatus)
        {
            try
            {
                if (!Enum.TryParse<CaseStatus>(newStatus, out var caseStatus))
                    return Result<PatientCaseDto>.Failure("Invalid status");

                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.Id == publicId));
                if (patientCase == null)
                    return Result<PatientCaseDto>.Failure("Case not found");

                if (!IsValidStatusTransition(patientCase.Status, caseStatus))
                    return Result<PatientCaseDto>.Failure($"Cannot change status from {patientCase.Status} to {caseStatus}");

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

        public async Task<Result<PatientCaseDto>> UpdateCaseWithStatusAsync(Guid publicId, string? treatmentType, CaseStatus newStatus)
        {
            try
            {
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.Id == publicId));
                if (patientCase == null)
                    return Result<PatientCaseDto>.Failure("Case not found");

                if (!IsValidStatusTransition(patientCase.Status, newStatus))
                    return Result<PatientCaseDto>.Failure($"Cannot change status from {patientCase.Status} to {newStatus}");

                patientCase.Status = newStatus;
                patientCase.UpdateAt = DateTime.UtcNow;
                _unitOfWork.PatientCases.Update(patientCase);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Case updated: {PublicId} - TreatmentType: {TreatmentType}, Status: {Status}",
                    publicId, treatmentType ?? "unchanged", newStatus);

                return await GetCaseByIdAsync(publicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating case with status: {PublicId}", publicId);
                return Result<PatientCaseDto>.Failure("Error updating case");
            }
        }

        private bool IsValidStatusTransition(CaseStatus currentStatus, CaseStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                (CaseStatus.Pending, CaseStatus.InProgress) => true,
                (CaseStatus.Pending, CaseStatus.Cancelled) => true,
                (CaseStatus.InProgress, CaseStatus.Completed) => true,
                (CaseStatus.InProgress, CaseStatus.Cancelled) => true,
                (CaseStatus.Completed, _) => false,
                (CaseStatus.Cancelled, _) => false,
                _ when currentStatus == newStatus => true,
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
                    return Result.Failure("Case can only be assigned to a university when it is in Pending or UnderReview status.", 400);

                Guid? targetUniversityId = universityId;

                if (role == "Student")
                {
                    var student = await _unitOfWork.Students.GetByIdAsync(new BaseSpecificationWithProjection<Student, Guid>(s => s.User.Id == userId, s => s.UniversityId));
                    if (student == Guid.Empty) return Result.Failure("Student profile not found", 404);
                    targetUniversityId = student;
                }
                else if (role == "Doctor")
                {
                    var doctor = await _unitOfWork.Doctors.GetByIdAsync(new BaseSpecificationWithProjection<Doctor, Guid>(d => d.User.Id == userId, d => d.UniversityId));
                    if (Guid.Empty == doctor) return Result.Failure("Doctor profile not found", 404);
                    targetUniversityId = doctor;
                }
                else if (role == "Admin")
                {
                    if (targetUniversityId == Guid.Empty || targetUniversityId == null)
                        return Result.Failure("Admin must provide a valid UniversityId.", 400);
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
