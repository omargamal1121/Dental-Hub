using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MediatR;
using DentalHub.Application.Queries.UniversityMember;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.UniversityMember;
using DentalHub.Application.Common;

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
        [ProducesResponseType(typeof(ApiResponse<PagedResult<UniversityMemberDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<UniversityMemberDto>>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null,
            [FromQuery] string? department = null)
        {
            var result = await _mediator.Send(new GetAllUniversityMembersQuery(page, pageSize, name, department));
            return HandleResult(result);
        }

        [HttpGet("{universityId}")]
        [ProducesResponseType(typeof(ApiResponse<UniversityMemberDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UniversityMemberDto>>> GetByUniversityId(string universityId)
        {
            var result = await _mediator.Send(new GetUniversityMemberByUniversityIdQuery(universityId));
            return HandleResult(result);
        }
    }
}
