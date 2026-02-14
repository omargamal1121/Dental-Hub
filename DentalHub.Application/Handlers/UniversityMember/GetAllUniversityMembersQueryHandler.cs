using DentalHub.Application.Common;
using DentalHub.Application.DTOs.UniversityMember;
using DentalHub.Application.Queries.UniversityMember;
using DentalHub.Application.Services.UniversityMembers;
using MediatR;

namespace DentalHub.Application.Handlers.UniversityMember
{
    public class GetAllUniversityMembersQueryHandler : IRequestHandler<GetAllUniversityMembersQuery, Result<PagedResult<UniversityMemberDto>>>
    {
        private readonly IUniversityMemberService _service;

        public GetAllUniversityMembersQueryHandler(IUniversityMemberService service)
        {
            _service = service;
        }

        public async Task<Result<PagedResult<UniversityMemberDto>>> Handle(GetAllUniversityMembersQuery request, CancellationToken ct)
        {
            return await _service.GetAllUniversityMembersAsync(request.Page, request.PageSize, request.Name, request.Department);
        }
    }
}
