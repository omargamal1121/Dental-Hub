using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Identity;
using DentalHub.Application.DTOs.UniversityMember;
using DentalHub.Application.Factories;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Specification;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Services.UniversityMembers
{
    public class UniversityMemberService : IUniversityMemberService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UniversityMemberService> _logger;

        public UniversityMemberService(IUnitOfWork unitOfWork, ILogger<UniversityMemberService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        // Get member by university ID
        public async Task<Result<UniversityMemberDto>> GetUniversityMemberByUniversityIdAsync(string universityId)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<UniversityMember, UniversityMemberDto>(
                    u => u.UniversityId == universityId,
                    u => new UniversityMemberDto
                    {
                        UniversityId = u.UniversityId,
                        Name = u.FullName,
                        Department = u.Department,
                        Role = u.Role
                    }
                );

                var member = await _unitOfWork.UniversityMembers.GetByIdAsync(spec);

                if (member == null)
                {
                    return Result<UniversityMemberDto>.Failure("University member not found", 404);
                }

                return Result<UniversityMemberDto>.Success(member);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting university member by university ID: {UniversityId}", universityId);
                return Result<UniversityMemberDto>.Failure("Error retrieving university member data", 500);
            }
        }

        // Register a student (role assigned automatically)
        // Register a student (role assigned automatically)
        public async Task<Result<UniversityMemberDto>> RegisterStudentAsync(RegisterStudentDto dto)
        {
            try
            {
                var student = new UniversityMember
                {
                    FullName = dto.FullName,
                    UniversityId = dto.UniversityId,
                    IsStudent = true
                };

                await _unitOfWork.UniversityMembers.AddAsync(student);
                await _unitOfWork.SaveChangesAsync();

                var dtoResult = new UniversityMemberDto
                {
                    UniversityId = student.UniversityId,
                    Name = student.FullName,
                    Department = student.Department,
                    Role = student.Role
                };

                return Result<UniversityMemberDto>.Success(dtoResult, "Student registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering student: {FullName}", dto.FullName);
                return Result<UniversityMemberDto>.Failure("Error registering student");
            }
        }

        // Register a doctor (role assigned automatically)
        public async Task<Result<UniversityMemberDto>> RegisterDoctorAsync(RegisterDoctorDto dto)
        {
            try
            {
                var doctor = new UniversityMember
                {
                    FullName = dto.FullName,
                    UniversityId = dto.UniversityId,
                    IsDoctor = true
                };

                await _unitOfWork.UniversityMembers.AddAsync(doctor);
                await _unitOfWork.SaveChangesAsync();

                var dtoResult = new UniversityMemberDto
                {
                    UniversityId = doctor.UniversityId,
                    Name = doctor.FullName,
                    Department = doctor.Department,
                    Role = doctor.Role
                };

                return Result<UniversityMemberDto>.Success(dtoResult, "Doctor registered successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering doctor: {FullName}", dto.FullName);
                return Result<UniversityMemberDto>.Failure("Error registering doctor");
            }
        }

        // Get all members with paging
        public async Task<Result<PagedResult<UniversityMemberDto>>> GetAllUniversityMembersAsync(int page = 1, int pageSize = 10, string? name = null, string? department = null)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<UniversityMember, UniversityMemberDto>(
                    u => (string.IsNullOrEmpty(name) || u.FullName.Contains(name)) &&
                         (string.IsNullOrEmpty(department) || u.Department.Contains(department)),
                    u => new UniversityMemberDto
                    {
                        UniversityId = u.UniversityId,
                        Name = u.FullName,
                        Department = u.Department,
                        Role = u.Role
                    }
                );

                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderBy(u => u.FullName);

                var membersList = await _unitOfWork.UniversityMembers.GetAllAsync(spec);
                var totalCount = await _unitOfWork.UniversityMembers.CountAsync(spec);

                var pagedResult = PaginationFactory<UniversityMemberDto>.Create(
                    count: totalCount,
                    page: page,
                    pageSize: pageSize,
                    data: membersList
                );

                return Result<PagedResult<UniversityMemberDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all university members");
                return Result<PagedResult<UniversityMemberDto>>.Failure("Error retrieving university members data", 500);
            }
        }
    }
}