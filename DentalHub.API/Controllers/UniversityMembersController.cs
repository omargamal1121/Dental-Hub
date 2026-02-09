using Microsoft.AspNetCore.Mvc;
using MediatR;
using DentalHub.Application.Queries.UniversityMember;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.UniversityMember;

namespace DentalHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversityMembersController : BaseController
    {
        private readonly IMediator _mediator;

        public UniversityMembersController(IMediator mediator) : base()
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UniversityMemberDto>>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null,
            [FromQuery] string? department = null)
        {
            var result = await _mediator.Send(new GetAllUniversityMembersQuery(page, pageSize, name, department));
            return HandleResult(result);
        }

        [HttpGet("{universityId}")]
        public async Task<ActionResult<ApiResponse<UniversityMemberDto>>> GetByUniversityId(string universityId)
        {
            var result = await _mediator.Send(new GetUniversityMemberByUniversityIdQuery(universityId));
            return HandleResult(result);
        }
    }
}
