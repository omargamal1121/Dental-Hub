using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.DTOs.Students;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Specification;
using DentalHub.Infrastructure.UnitOfWork;
using DentalHub.Application.Factories;
using Microsoft.Extensions.Logging;

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

        public async Task<Result<StudentDto>> GetStudentByIdAsync(Guid userId)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Student, StudentDto>(
                    s => s.UserId == userId,
                    s => new StudentDto
                    {
                        UserId = s.UserId,
                        FullName = s.User.FullName,
                        Email = s.User.Email!,
                        University = s.University,
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
                _logger.LogError(ex, "Error getting student: {UserId}", userId);
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
                        UserId = s.UserId,
                        FullName = s.User.FullName,
                        Email = s.User.Email!,
                        University = s.University,
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
                var spec = new BaseSpecification<Student>(s => s.UserId == dto.UserId);
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

                _logger.LogInformation("Student updated successfully: {UserId}", dto.UserId);

                return await GetStudentByIdAsync(dto.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating student: {UserId}", dto.UserId);
                return Result<StudentDto>.Failure("Error updating student");
            }
        }

  
        public async Task<Result> DeleteStudentAsync(Guid userId)
        {
            try
            {
                var student = await _unitOfWork.Students.GetByIdAsync(
                    new BaseSpecification<Student>(s => s.UserId == userId));

                if (student == null)
                {
                    return Result.Failure("Student not found");
                }

                student.DeleteAt = DateTime.UtcNow;
                //_unitOfWork.Students.Update(student);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Student deleted: {UserId}", userId);

                return Result.Success("Student deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting student: {UserId}", userId);
                return Result.Failure("Error deleting student");
            }
        }

        #endregion

        #region Available Cases

     
        public async Task<Result<PagedResult<PatientCaseDto>>> GetAvailableCasesForStudentAsync(
            Guid studentId, int page = 1, int pageSize = 10)
        {
            try
            {
                var student = await _unitOfWork.Students.GetByIdAsync(
                    new BaseSpecification<Student>(s => s.UserId == studentId));

                if (student == null)
                {
                    return Result<PagedResult<PatientCaseDto>>.Failure("Student not found");
                }

                var spec = new BaseSpecificationWithProjection<PatientCase, PatientCaseDto>(
                    pc => pc.Status == CaseStatus.Pending,
                    pc => new PatientCaseDto
                    {
                        Id = pc.Id,
                        PatientId = pc.PatientId,
                        PatientName = pc.Patient.User.FullName,
                        PatientAge = pc.Patient.Age,
                       
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
                _logger.LogError(ex, "Error getting available cases for student: {StudentId}", studentId);
                return Result<PagedResult<PatientCaseDto>>.Failure("Error retrieving available cases");
            }
        }

        #endregion

        #region Statistics

        public async Task<Result<StudentStatsDto>> GetStudentStatisticsAsync(Guid studentId)
        {
            try
            {
                var spec = new BaseSpecification<Student>(s => s.UserId == studentId);
               

                var student = await _unitOfWork.Students.GetByIdAsync(spec);

                if (student == null)
                {
                    return Result<StudentStatsDto>.Failure("Student not found");
                }

              
                var sessionsSpec = new BaseSpecification<Session>(s => s.StudentId == studentId);
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
                _logger.LogError(ex, "Error getting student statistics: {StudentId}", studentId);
                return Result<StudentStatsDto>.Failure("Error retrieving statistics");
            }
        }

        #endregion
    }
}
