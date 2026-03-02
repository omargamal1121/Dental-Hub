using DentalHub.Application.Common;
using DentalHub.Application.DTOs.UniversityMember;
using MediatR;

namespace DentalHub.Application.Queries.UniversityMember
{
    public record GetUniversityMemberByUniversityIdQuery(Guid UniversityId) : IRequest<Result<UniversityMemberDto>>;
}
