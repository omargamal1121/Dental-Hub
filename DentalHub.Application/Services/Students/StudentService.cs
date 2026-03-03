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

        public async Task<Result<StudentDto>> GetStudentByPublicIdAsync(string publicId)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Student, StudentDto>(
                    s => s.PublicId == publicId,
                    s => new StudentDto
                    {
                        PublicId = s.PublicId,
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
                    return Result<StudentDto>.Failure("Student not found");
                }

                return Result<StudentDto>.Success(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student by public ID: {PublicId}", publicId);
                return Result<StudentDto>.Failure("Error retrieving student data");
            }
        }

        
        public async Task<Result<StudentDto>> GetStudentByUserIdAsync(string userId)
        {
            try
            {
          
                var spec = new BaseSpecificationWithProjection<Student, StudentDto>(
                    s => s.PublicId == userId,
                    s => new StudentDto
                    {
                        PublicId = s.PublicId,
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
                    return Result<StudentDto>.Failure("Student not found");

                return Result<StudentDto>.Success(student);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student by user ID: {UserId}", userId);
                return Result<StudentDto>.Failure("Error retrieving student data");
            }
        }


        public async Task<Result<PagedResult<StudentDto>>> GetAllStudentsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Student, StudentDto>(
                    s => new StudentDto
                    {
                        PublicId = s.PublicId,
                        FullName = s.User.FullName,
                        Email = s.User.Email!,
                        UniversityId = s.UniversityId,
                        Level = s.Level,
                        CreateAt = s.CreateAt,
                        TotalRequests = s.CaseRequests.Count,
                        ApprovedRequests = s.CaseRequests.Count(cr => cr.Status == RequestStatus.Approved)
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


        public async Task<Result<StudentDto>> UpdateStudentAsync(UpdateStudentDto dto)
        {
            try
            {
                var spec = new BaseSpecification<Student>(s => s.PublicId == dto.PublicId);
                spec.AddInclude(s => s.User);

                var student = await _unitOfWork.Students.GetByIdAsync(spec);

                if (student == null)
                {
                    return Result<StudentDto>.Failure("Student not found");
                }


                if (!string.IsNullOrWhiteSpace(dto.FullName))
                {
                    student.User.FullName = dto.FullName;
                }

                if (!string.IsNullOrWhiteSpace(dto.University))
                {
                    student.University = dto.University;
                }

                if (dto.Level.HasValue)
                {
                    student.Level = dto.Level.Value;
                }

                student.UpdateAt = DateTime.UtcNow;

                _unitOfWork.Students.Update(student);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Student updated successfully: {PublicId}", dto.PublicId);

                return await GetStudentByPublicIdAsync(dto.PublicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student: {PublicId}", dto.PublicId);
                return Result<StudentDto>.Failure("Error updating student");
            }
        }


        public async Task<Result> DeleteStudentByPublicIdAsync(string publicId)
        {
            try
            {
                var student = await _unitOfWork.Students.GetByIdAsync(
                    new BaseSpecification<Student>(s => s.PublicId == publicId));

                if (student == null)
                {
                    return Result.Failure("Student not found");
                }

                student.DeleteAt = DateTime.UtcNow;
                //_unitOfWork.Students.Update(student);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Student deleted: {PublicId}", publicId);

                return Result.Success("Student deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student by public ID: {PublicId}", publicId);
                return Result.Failure("Error deleting student");
            }
        }

        #endregion

        #region Cases

        /// Returns cases on which the student has already placed at least one CaseRequest.
        public async Task<Result<PagedResult<PatientCaseDto>>> GetMyCasesForStudentAsync(
            string studentPublicId,string Casetype ,int page = 1, int pageSize = 10)
        {
            try
            {
             
                var studentid = await _unitOfWork.Students.GetByIdAsync(
                    new BaseSpecificationWithProjection<Student,Guid>(s => s.PublicId == studentPublicId,s=>s.Id));

                if (Guid.Empty== studentid)
                    return Result<PagedResult<PatientCaseDto>>.Failure("Student not found", 404);

                var studentGuid = studentid;
 
                var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                    pc => pc.CaseRequests.Any(cr => cr.StudentId == studentGuid&&cr.Status==RequestStatus.Approved)&&(
                    string.IsNullOrEmpty(Casetype)||pc.CaseType.Name.Contains(Casetype)||pc.CaseType.Description.Contains(Casetype)),
                    pc => new PatientCaseDto
                    {
                        Id              = pc.PublicId,
                        PatientId       = pc.Patient.PublicId,
                        PatientName     = pc.Patient.User.FullName,
                        PatientAge      = pc.Patient.Age,
                        CaseType        = new DTOs.CaseTypes.CaseTypeDto
                        {
                            publicId    = pc.CaseType.PublicId,
                            Name        = pc.CaseType.Name,
                            Description = pc.CaseType.Description
                        },
                        Status          = pc.Status.ToString(),
                        CreateAt        = pc.CreateAt,
                        TotalSessions   = pc.Sessions.Count,
                        PendingRequests = pc.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending),
                        ImageUrls       = pc.Medias.Select(m => m.MediaUrl).ToList()
                    }
                );

                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(pc => pc.CreateAt);

                var casesList  = await _unitOfWork.PatientCases.GetAllAsync(spec);
                var totalCount = await _unitOfWork.PatientCases.CountAsync(spec);

                var pagedResult = PaginationFactory<PatientCaseDto>.Create(
                    count:    totalCount,
                    page:     page,
                    pageSize: pageSize,
                    data:     casesList
                );

                return Result<PagedResult<PatientCaseDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting my cases for student: {StudentId}", studentPublicId);
                return Result<PagedResult<PatientCaseDto>>.Failure("Error retrieving cases");
            }
        }

      
        public async Task<Result<PagedResult<AvailableCasesDto>>> GetAvailableCasesForStudentAsync(
            string studentPublicId,string? CaseName=null, int page = 1, int pageSize = 10)
        {
            try
            {
                var studentid = await _unitOfWork.Students.GetByIdAsync(
                    new BaseSpecificationWithProjection<Student,Guid>(s => s.PublicId == studentPublicId,s=>s.Id));

                if (studentid==Guid.Empty)
                    return Result<PagedResult<AvailableCasesDto>>.Failure("Student not found", 404);

                var studentGuid = studentid;

                
                var spec = new BaseSpecificationWithProjection<PatientCase, AvailableCasesDto>(
                    pc => pc.Status == CaseStatus.Pending &&
                          !pc.CaseRequests.Any(cr => cr.StudentId == studentGuid)
                          &&(
                          string.IsNullOrEmpty(CaseName) ||
						  pc.CaseType.Name.Contains(CaseName)||pc.CaseType.Description.Contains(CaseName)),
                    pc => new AvailableCasesDto
                    {
                        Id              = pc.PublicId,
                        PatientId       = pc.Patient.PublicId,
                        PatientName     = pc.Patient.User.FullName,
                        PatientAge      = pc.Patient.Age,
                        CaseType        = new DTOs.CaseTypes.CaseTypeDto
                        {
                            publicId    = pc.CaseType.PublicId,
                            Name        = pc.CaseType.Name,
                            Description = pc.CaseType.Description
                        },
                        Status          = pc.Status.ToString(),
                        CreateAt        = pc.CreateAt,
                        ImageUrls       = pc.Medias.Select(m => m.MediaUrl).ToList()
                    }
                );

                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(pc => pc.CreateAt);

                var casesList  = await _unitOfWork.PatientCases.GetAllAsync(spec);
                var totalCount = await _unitOfWork.PatientCases.CountAsync(spec);

                var pagedResult = PaginationFactory<AvailableCasesDto>.Create(
                    count:    totalCount,
                    page:     page,
                    pageSize: pageSize,
                    data:     casesList
                );

                return Result<PagedResult<AvailableCasesDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting available cases for student: {StudentId}", studentPublicId);
                return Result<PagedResult<AvailableCasesDto>>.Failure("Error retrieving available cases");
            }
        }

        #endregion

        #region Statistics

        public async Task<Result<StudentStatsDto>> GetStudentStatisticsAsync(string studentPublicId)
        {
            try
            {
                var spec = new BaseSpecification<Student>(s => s.PublicId == studentPublicId);


                var student = await _unitOfWork.Students.GetByIdAsync(spec);

                if (student == null)
                {
                    return Result<StudentStatsDto>.Failure("Student not found");
                }


                var sessionsSpec = new BaseSpecification<Session>(s => s.Student.PublicId == studentPublicId);
                var sessions = await _unitOfWork.Sessions.GetAllAsync(sessionsSpec);


                var approvedRequests = student.CaseRequests
                    .Where(cr => cr.Status == RequestStatus.Approved)
                    .ToList();

                var stats = new StudentStatsDto
                {
                    TotalRequests = student.CaseRequests.Count,
                    PendingRequests = student.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending),
                    ApprovedRequests = approvedRequests.Count,
                    RejectedRequests = student.CaseRequests.Count(cr => cr.Status == RequestStatus.Rejected),
                    TotalSessions = sessions.Count,
                    CompletedSessions = sessions.Count(s => s.Status == SessionStatus.Done),
                    TotalCases = approvedRequests.Count
                };

                return Result<StudentStatsDto>.Success(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting student statistics for public ID: {PublicId}", studentPublicId);
                return Result<StudentStatsDto>.Failure("Error retrieving statistics");
            }
        }

        #endregion
    }
}
