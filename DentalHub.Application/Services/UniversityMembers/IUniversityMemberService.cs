using DentalHub.Application.Common;
using DentalHub.Application.DTOs.UniversityMember;

namespace DentalHub.Application.Services.UniversityMembers
{
    public interface IUniversityMemberService
    {
        Task<Result<UniversityMemberDto>> GetUniversityMemberByUniversityIdAsync(string universityId);
        Task<Result<PagedResult<UniversityMemberDto>>> GetAllUniversityMembersAsync(int page = 1, int pageSize = 10, string? name = null, string? department = null);
    }
}
