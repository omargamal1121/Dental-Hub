using DentalHub.Application.Common;
using DentalHub.Application.DTOs.UniversityMember;
using MediatR;

namespace DentalHub.Application.Queries.UniversityMember
{
    public record GetAllUniversityMembersQuery(int Page = 1, int PageSize = 10, string? Name = null, string? Department = null) : IRequest<Result<PagedResult<UniversityMemberDto>>>;
}
