using DentalHub.Application.Common;
using DentalHub.Application.DTOs.UniversityMember;
using DentalHub.Application.Queries.UniversityMember;
using DentalHub.Application.Services.UniversityMembers;
using MediatR;

namespace DentalHub.Application.Handlers.UniversityMember
{
    public class GetUniversityMemberByUniversityIdQueryHandler : IRequestHandler<GetUniversityMemberByUniversityIdQuery, Result<UniversityMemberDto>>
    {
        private readonly IUniversityMemberService _service;

        public GetUniversityMemberByUniversityIdQueryHandler(IUniversityMemberService service)
        {
            _service = service;
        }

        public async Task<Result<UniversityMemberDto>> Handle(GetUniversityMemberByUniversityIdQuery request, CancellationToken ct)
        {
            return await _service.GetUniversityMemberByUniversityIdAsync(request.UniversityId);
        }
    }
}
