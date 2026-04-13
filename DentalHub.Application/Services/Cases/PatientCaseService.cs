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
             
                var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                    pc => pc.Id == publicId,
                    PatientCaseProjections.ToDto
                );

                var dto = await _unitOfWork.PatientCases.GetByIdAsync(spec);
                if (dto == null)
                    return Result<PatientCaseDto>.Failure("Case not found", 404);

                // ── 3. Compute ProcessStatus ──
                dto.ProcessStatus = ComputeProcessStatus(dto, dto.Diagnosisdto);

                // ── 4. Compute UserFlags ──
                if (userId.HasValue && !string.IsNullOrEmpty(userRole))
                {
                    dto.UserFlags = await ComputeUserFlagsAsync(dto, userId.Value, dto.PatientId, dto.Id, userRole);
                }

                // ── 5. Compute AvailableActions ──
                dto.AvailableActions = ComputeAvailableActions(dto.UserFlags, dto);

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
        private static string ComputeProcessStatus(PatientCaseDto dto, Diagnosisdto? diagnosisDto)
        {
            if (dto.Status == CaseStatus.Completed.ToString())
                return "Completed";

            if (dto.Status == CaseStatus.InProgress.ToString())
            {
                return dto.HasEvaluatedSession ? "Evaluated" : "InProgress";
            }

            // Pending or UnderReview:
            // Priority: UnAssigned first (no student), then diagnosis stage, then raw status
            if (dto.AssignedStudentId == null)
                return "UnAssigned";

            if (diagnosisDto != null)
            {
                if (diagnosisDto.DiagnosisStage == DiagnosisStage.BasicClinic.ToString() ||
                    diagnosisDto.DiagnosisStage == DiagnosisStage.AdvancedClinic.ToString())
                    return "DiagnosedInClinic";

                if (diagnosisDto.DiagnosisStage == DiagnosisStage.AI.ToString())
                    return "AIPreliminaryDiagnosis";
            }

            return dto.Status.ToString();
        }

        /// <summary>
        /// Resolve who the current user is relative to this case and return flags.
        /// </summary>
        private async Task<CaseUserFlags> ComputeUserFlagsAsync(PatientCaseDto dto, Guid userId,Guid patientId,Guid patientCaseId, string userRole)
        {
            var flags = new CaseUserFlags
            {
                Role= userRole
            };
            flags.IsAssignedStudent = dto.AssignedStudentId.HasValue;
            flags.IsAssignedDoctor = dto.AssignedDoctorId.HasValue;
            


			switch (userRole)
            {
                case "Patient":
                    {

                        flags.IsOwner = (patientId != Guid.Empty && patientId == dto.PatientId);
                        break;
                    }

                case "Doctor":
                    {
						flags.IsOwner = (userId != Guid.Empty && userId == dto.AssignedDoctorId);
                        flags.IsAssignedToMe = (userId != Guid.Empty && userId == dto.AssignedDoctorId);




						var requestSpec = new BaseSpecificationWithProjection<CaseRequest, RequestDataDto>(
                            cr => cr.PatientCaseId == dto.Id && cr.DoctorId == userId,cr=>new RequestDataDto
							{
                                Id = cr.Id,
                                Status = cr.Status,
                                
                            });



                        var request = await _unitOfWork.CaseRequests.GetByIdAsync(requestSpec);
						if (request != null)
						{
							flags.HasRequest = true;
							flags.RequestStatus = request.Status.ToString();
							flags.RequestId = request.Id;

						}
                        else
                            flags.HasRequest = false;
						break;
                    }

                case "Student":
                    {
                       

                        flags.IsAssignedToMe = (userId != Guid.Empty && userId == dto.AssignedStudentId);
                        flags.IsOwner= (userId != Guid.Empty && userId == dto.AssignedStudentId);


						var requestSpec = new BaseSpecificationWithProjection<CaseRequest, RequestDataDto>(
                            cr => cr.PatientCaseId == dto.Id && cr.Student.User.Id == userId, cr => new RequestDataDto
                            {
                                Status = cr.Status,
                                 Id= cr.Id,
                                

                            });
                        var request = await _unitOfWork.CaseRequests.GetByIdAsync(requestSpec);

                        if (request != null) 
                        {
                            flags.HasRequest = true;
							flags.RequestStatus = request.Status.ToString();
                            flags.RequestId = request.Id;

						}
                        else
                        {
                            flags.HasRequest = false;
                        }   

                        break;
                    }
            }

            return flags;
        }

        /// <summary>
        /// Determine what actions the current user can perform on the case.
        /// </summary>
        private static List<string> ComputeAvailableActions(CaseUserFlags flags, PatientCaseDto dto)
        {
            var actions = new List<string>();

            switch (flags.Role)
            {
                case "Patient":
                    {
                       
                        if (flags.IsOwner && dto.Status == CaseStatus.Pending.ToString())
                            actions.Add("Remove");
         
                        if (flags.IsOwner)
                        {
                            actions.Add("ViewSessions");
                            actions.Add("ViewDiagnosis");
                        }
                        break;
					}
                    case "Doctor":
                    {
                     

                        if (flags.IsAssignedToMe)
                        {
                            if (!actions.Contains("ViewCase")) actions.Add("ViewCase");
                            actions.Add("ViewSessions");
                            actions.Add("EvaluateSession");
                            actions.Add("AddDiagnosis");
                            if (dto.Status == CaseStatus.InProgress.ToString())
                                actions.Add("CompleteCase");
                        }
                     
                        else if (flags.HasRequest && flags.RequestStatus == RequestStatus.Pending.ToString())
                        {
                            if (dto.Status == CaseStatus.Pending.ToString() || !dto.AssignedStudentId.HasValue)
                            {
                                actions.Add("AcceptRequest");
                            }
                            actions.Add("RejectRequest");
                        }
                        break;
					}
                    case "Student":
                    {
                       

                        if (flags.IsAssignedToMe)
                        {
                            if (!actions.Contains("ViewCase")) actions.Add("ViewCase");
                            actions.Add("ViewSessions");
                            actions.Add("CreateSession");
                            actions.Add("AddSessionNote");
                        }
                        else if (!flags.HasRequest &&
                                 (dto.Status == CaseStatus.Pending.ToString() || dto.Status == CaseStatus.UnderReview.ToString()) &&
                                 dto.IsPublic)
                        {
                            actions.Add("RequestCase");
                        }
                        else if (flags.HasRequest && flags.RequestStatus == RequestStatus.Pending.ToString ())
                        {
                            actions.Add("CancelRequest");
                        }
                        break;
					}
				default:
                    break;
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

        public async Task<Result<PagedResult<PatientCaseDto>>> GetAllCasesAsync(CaseFilterDto filter)
        {
            try
            {
                CaseStatus? parsedStatus = null;
                if (!string.IsNullOrWhiteSpace(filter.Status))
                {
                    if (!Enum.TryParse<CaseStatus>(filter.Status, ignoreCase: true, out var s))
                        return Result<PagedResult<PatientCaseDto>>.Failure(
                            $"Invalid status '{filter.Status}'. Valid values: {string.Join(", ", Enum.GetNames<CaseStatus>())}", 400);
                    parsedStatus = s;
                }

                var nameFilter = filter.PatientName?.Trim().ToLower();
                var caseTypeFilter = filter.CaseType?.Trim().ToLower();

                var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                    criteria: pc =>
                        (parsedStatus == null || pc.Status == parsedStatus) &&
                        (nameFilter == null || pc.Patient.User.FullName.ToLower().StartsWith(nameFilter)) &&
                        (caseTypeFilter == null || pc.Diagnosiss.Any(d => d.CaseType.Name.ToLower().StartsWith(caseTypeFilter))) &&
                        (filter.Gender == null || pc.Patient.Gender == filter.Gender),
                    PatientCaseProjections.ToDto
                );

                spec.ApplyPaging(filter.Page, filter.PageSize);

                if (!string.IsNullOrEmpty(filter.SortBy))
                {
                    bool isDesc = filter.SortDirection?.ToLower() == "desc";
                    switch (filter.SortBy.ToLower())
                    {
                        case "name":
                            if (isDesc) spec.ApplyOrderByDescending(pc => pc.Patient.User.FullName);
                            else spec.ApplyOrderBy(pc => pc.Patient.User.FullName);
                            break;
                        case "age":
                            if (isDesc) spec.ApplyOrderByDescending(pc => pc.Patient.Age);
                            else spec.ApplyOrderBy(pc => pc.Patient.Age);
                            break;
                        case "date":
                        default:
                            if (isDesc) spec.ApplyOrderByDescending(pc => pc.CreateAt);
                            else spec.ApplyOrderBy(pc => pc.CreateAt);
                            break;
                    }
                }
                else
                {
                    spec.ApplyOrderByDescending(pc => pc.CreateAt);
                }

                // Separate count spec without paging to get correct total
                var countSpec = new BaseSpecification<PatientCase>(
                    pc =>
                        (parsedStatus == null || pc.Status == parsedStatus) &&
                        (nameFilter == null || pc.Patient.User.FullName.ToLower().StartsWith(nameFilter)) &&
                        (caseTypeFilter == null || pc.Diagnosiss.Any(d => d.CaseType.Name.ToLower().StartsWith(caseTypeFilter))) &&
                        (filter.Gender == null || pc.Patient.Gender == filter.Gender));

                var casesList = await _unitOfWork.PatientCases.GetAllAsync(spec);
                var totalCount = await _unitOfWork.PatientCases.CountAsync(countSpec);

                var pagedResult = PaginationFactory<PatientCaseDto>.Create(
                    count: totalCount, page: filter.Page, pageSize: filter.PageSize, data: casesList);

                return Result<PagedResult<PatientCaseDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all cases");
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
    public class RequestDataDto
    {
        public Guid Id { get; set; }
        public RequestStatus Status { get; set; }
        

    }
}
