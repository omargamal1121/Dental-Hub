using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.DTOs.Students;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using DentalHub.Application.Factories;
using Microsoft.Extensions.Logging;
using DentalHub.Application.Specification.Comman;

namespace DentalHub.Application.Services.Students
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StudentService> _logger;

        public StudentService(IUnitOfWork unitOfWork, ILogger<StudentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Student Profile

        public async Task<Result<StudentDetailsDto>> GetStudentByIdAsync(Guid id)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Student, StudentDetailsDto>(
                    s => s.Id == id,
                    s => new StudentDetailsDto
                    {
                        PublicId = s.Id,
                        FullName = s.User.FullName,
                        Email = s.User.Email!,
                        UniversityId = s.UniversityId,
                        Level = s.Level,
                        CreateAt = s.CreateAt,
                        TotalRequests = s.CaseRequests.Count,
                        ApprovedRequests = s.CaseRequests.Count(cr => cr.Status == RequestStatus.Approved)
                    }
                );



                var student = await _unitOfWork.Students.GetByIdAsync(spec);

                if (student == null)
                {
                    return Result<StudentDetailsDto>.Failure("Student not found");
                }

                return Result<StudentDetailsDto>.Success(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student by ID: {Id}", id);
                return Result<StudentDetailsDto>.Failure("Error retrieving student data");
            }
        }


        public async Task<Result<StudentDetailsDto>> GetStudentByUserIdAsync(Guid userId)
        {
            try
            {

                var spec = new BaseSpecificationWithProjection<Student, StudentDetailsDto>(
                    s => s.Id == userId,
                    s => new StudentDetailsDto
                    {
                        PublicId = s.Id,
                        FullName = s.User.FullName,
                        Email = s.User.Email!,
                        UniversityId = s.UniversityId,
                        Level = s.Level,
                        CreateAt = s.CreateAt,
                        TotalRequests = s.CaseRequests.Count,
                        ApprovedRequests = s.CaseRequests.Count(cr => cr.Status == RequestStatus.Approved)
                    }
                );

                var student = await _unitOfWork.Students.GetByIdAsync(spec);

                if (student == null)
                    return Result<StudentDetailsDto>.Failure("Student not found");

                return Result<StudentDetailsDto>.Success(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student by user ID: {UserId}", userId);
                return Result<StudentDetailsDto>.Failure("Error retrieving student data");
            }
        }


        public async Task<Result<PagedResult<StudentDto>>> GetAllStudentsAsync(int page = 1, int pageSize = 10, string? search = null, int? level = null)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Student, StudentDto>(
                    s => (string.IsNullOrEmpty(search) || s.User.FullName.Contains(search) || (s.User.Email != null && s.User.Email.Contains(search))) &&
                         (!level.HasValue || s.Level == level.Value),
                    s => new StudentDto
                    {
                        PublicId = s.Id,
                        FullName = s.User.FullName,
                        Email = s.User.Email!,
                        UniversityId = s.UniversityId,
                        Level = s.Level,
                        CreateAt = s.CreateAt,
                    }
                );


                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(s => s.CreateAt);

                var studentsList = await _unitOfWork.Students.GetAllAsync(spec);
                var totalCount = await _unitOfWork.Students.CountAsync(spec);

                var pagedResult = PaginationFactory<StudentDto>.Create(
                    count: totalCount,
                    page: page,
                    pageSize: pageSize,
                    data: studentsList
                );

                return Result<PagedResult<StudentDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all students");
                return Result<PagedResult<StudentDto>>.Failure("Error retrieving students");
            }
        }


        public async Task<Result<StudentDetailsDto>> UpdateStudentAsync(UpdateStudentDto dto)
        {
            try
            {
                var spec = new BaseSpecification<Student>(s => s.Id == dto.PublicId);
                spec.AddInclude(s => s.User);

                var student = await _unitOfWork.Students.GetByIdAsync(spec);

                if (student == null)
                {
                    return Result<StudentDetailsDto>.Failure("Student not found");
                }


                if (!string.IsNullOrWhiteSpace(dto.FullName))
                {
                    student.User.FullName = dto.FullName;
                }

                //if (!string.IsNullOrWhiteSpace(dto.University))
                //{
                //    student.University = dto.University;
                //}

                if (dto.Level.HasValue)
                {
                    student.Level = dto.Level.Value;
                }

                student.UpdateAt = DateTime.UtcNow;

                _unitOfWork.Students.Update(student);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Student updated successfully: {Id}", dto.PublicId);

                return await GetStudentByIdAsync(dto.PublicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student: {PublicId}", dto.PublicId);
                return Result<StudentDetailsDto>.Failure("Error updating student");
            }
        }


        public async Task<Result> DeleteStudentByIdAsync(Guid id)
        {
            try
            {
                var student = await _unitOfWork.Students.GetByIdAsync(
                    new BaseSpecification<Student>(s => s.Id == id));

                if (student == null)
                {
                    return Result.Failure("Student not found");
                }

                student.DeleteAt = DateTime.UtcNow;
                //_unitOfWork.Students.Update(student);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Student deleted: {Id}", id);

                return Result.Success("Student deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student by public ID: {PublicId}", id);
                return Result.Failure("Error deleting student");
            }
        }

        #endregion

        #region Cases

        /// Returns cases on which the student has already placed at least one CaseRequest.
        public async Task<Result<PagedResult<PatientCaseDto>>> GetMyCasesForStudentAsync(
            Guid studentId, string Casetype, int page = 1, int pageSize = 10)
        {
            try
            {

                var studentGuid = studentId;
                var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                    pc => pc.CaseRequests.Any(cr => cr.StudentId == studentGuid && cr.Status == RequestStatus.Approved),
                    //&& (string.IsNullOrEmpty(Casetype) || pc.CaseType.Name.Contains(Casetype) || pc.CaseType.Description.Contains(Casetype)),
                    pc => new PatientCaseDto
                    {
                        Id = pc.Id,
                        PatientId = pc.Patient.Id,
                        PatientName = pc.Patient.User.FullName,
                        PatientAge = pc.Patient.Age,
                        //CaseType = new DTOs.CaseTypes.CaseTypeDto
                        //{
                        //    publicId = pc.CaseType.Id,
                        //    Name = pc.CaseType.Name,
                        //    Description = pc.CaseType.Description
                        //},
                        Status = pc.Status.ToString(),
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
                _logger.LogError(ex, "Error getting my cases for student: {StudentId}", studentId);
                return Result<PagedResult<PatientCaseDto>>.Failure("Error retrieving cases");
            }
        }


        public async Task<Result<PagedResult<AvailableCasesDto>>> GetAvailableCasesForStudentAsync(
            Guid studentId, CaseFilterDto filter)
        {
            try
            {
                var studentGuid = studentId;

                var nameFilter = filter.PatientName?.Trim().ToLower();
                var caseTypeFilter = filter.CaseType?.Trim().ToLower();

                var spec = new BaseSpecificationWithProjection<PatientCase, AvailableCasesDto>(
                    pc => pc.Status == CaseStatus.Pending &&
                          !pc.CaseRequests.Any(cr => cr.StudentId == studentGuid) &&
                          (nameFilter == null || pc.Patient.User.FullName.ToLower().StartsWith(nameFilter)) &&
                          (caseTypeFilter == null || pc.Diagnosiss.Any(d => d.CaseType.Name.ToLower().StartsWith(caseTypeFilter))) &&
                          (filter.Gender == null || pc.Patient.Gender == filter.Gender),
                    pc => new AvailableCasesDto
                    {
                        Id = pc.Id,
                        PatientId = pc.Patient.Id,
                        PatientName = pc.Patient.User.FullName,
                        PatientAge = pc.Patient.Age,
                        Status = pc.Status.ToString(),
                        CreateAt = pc.CreateAt,
                        ImageUrls = pc.Medias.Select(m => m.MediaUrl).ToList(),
                        gender = pc.Patient.Gender,
                        Diagnosisdto = pc.Diagnosiss
                        
                        
                        
        .Select(d => new Diagnosisdto
        {
            Id = d.Id,
            Notes = d.Notes,
            CaseType = d.CaseType.Name,
            DiagnosisStage = d.Stage.ToString(),
            TeethNumbers = d.TeethNumbers
        }).ToList(),
                    }
                );
                {

                };
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

                var countSpec = new BaseSpecification<PatientCase>(
                    pc => pc.Status == CaseStatus.Pending &&
                          !pc.CaseRequests.Any(cr => cr.StudentId == studentGuid) &&
                          (nameFilter == null || pc.Patient.User.FullName.ToLower().StartsWith(nameFilter)) &&
                          (caseTypeFilter == null || pc.Diagnosiss.Any(d => d.CaseType.Name.ToLower().StartsWith(caseTypeFilter))) &&
                          (filter.Gender == null || pc.Patient.Gender == filter.Gender)
                );

                var casesList = await _unitOfWork.PatientCases.GetAllAsync(spec);
                var totalCount = await _unitOfWork.PatientCases.CountAsync(countSpec);

                var pagedResult = PaginationFactory<AvailableCasesDto>.Create(
                    count: totalCount,
                    page: filter.Page,
                    pageSize: filter.PageSize,
                    data: casesList
                );

                return Result<PagedResult<AvailableCasesDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available cases for student: {StudentId}", studentId);
                return Result<PagedResult<AvailableCasesDto>>.Failure("Error retrieving available cases");
            }
        }

        #endregion

        #region Statistics

        public async Task<Result<StudentStatsDto>> GetStudentStatisticsAsync(Guid studentId)
        {
            try
            {
                var spec = new BaseSpecification<Student>(s => s.Id == studentId);
                spec.AddInclude(s => s.CaseRequests);

                var student = await _unitOfWork.Students.GetByIdAsync(spec);

                if (student == null)
                    return Result<StudentStatsDto>.Failure("Student not found", 404);

                var sessionsSpec = new BaseSpecification<Session>(s => s.StudentId == studentId);
                var sessions = await _unitOfWork.Sessions.GetAllAsync(sessionsSpec);

                var stats = new StudentStatsDto
                {
                    TotalRequests = student.CaseRequests.Count,
                    PendingRequests = student.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending),
                    ApprovedRequests = student.CaseRequests.Count(cr => cr.Status == RequestStatus.Approved),
                    RejectedRequests = student.CaseRequests.Count(cr => cr.Status == RequestStatus.Rejected),
                    TotalSessions = sessions.Count,
                    CompletedSessions = sessions.Count(s => s.Status == SessionStatus.Done),
                    TotalCases = student.CaseRequests.Count(cr => cr.Status == RequestStatus.Approved)
                };

                return Result<StudentStatsDto>.Success(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student statistics for ID: {Id}", studentId);
                return Result<StudentStatsDto>.Failure("Error retrieving statistics");
            }
        }

        public async Task<Result> HandleBeforeDeleteAsync(Guid id)
        {
            try
            {
                _logger.LogInformation("Attempting to handle student before delete: {Id}", id);
                var student = await _unitOfWork.Students.GetByIdAsync(
                    new BaseSpecificationWithProjection<Student, GetStudentDataById>(s => s.Id == id, s =>
                    new GetStudentDataById
                    {
                        Id = s.Id,
                        HasProgressCases = s.CaseRequests.Any(cr => cr.Status == RequestStatus.Approved && cr.PatientCase.Status == CaseStatus.InProgress)
                    }));

                if (student == null)
                    return Result.Failure("Student not found");
                if (student.HasProgressCases)
                    return Result.Failure("Cannot delete student with in-progress cases");

                await _unitOfWork.CaseRequests.CancelAllStudentRequestsAsync(student.Id);

                
                _logger.LogInformation("Student deleted preparation done: {Id}", id);
                return Result.Success("Student pre-delete handling successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling before delete for student: {Id}", id);
                return Result.Failure("Error handling before delete for student");
            }
        }

        #endregion
    }

    public class GetStudentDataById
    {
        public Guid Id { get; set; }
        public bool HasProgressCases { get; set; }
    }
}
