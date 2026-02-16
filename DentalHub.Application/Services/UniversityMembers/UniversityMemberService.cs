using DentalHub.Application.Common;
using DentalHub.Application.DTOs.UniversityMember;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Specification;
using DentalHub.Infrastructure.UnitOfWork;
using DentalHub.Application.Factories;
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
