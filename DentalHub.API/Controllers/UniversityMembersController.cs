using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MediatR;
using DentalHub.Application.Queries.UniversityMember;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.UniversityMember;
using DentalHub.Application.Common;
using Swashbuckle.AspNetCore.Annotations;

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

        /// <summary>
        /// Get all university members (Doctor and Student registry).
        /// Use the universityId when creating a Doctor or Student.
        /// </summary>
        /// <remarks>
        /// All universities support both Doctor and Student roles:
        ///
        /// | University            | UniversityId                         |
        /// |-----------------------|--------------------------------------|
        /// | Cairo University      | 11111111-1111-1111-1111-111111111111 |
        /// | Ain Shams University  | 22222222-2222-2222-2222-222222222222 |
        /// | Mansoura University   | 33333333-3333-3333-3333-333333333333 |
        /// | Alexandria University | 44444444-4444-4444-4444-444444444444 |
        /// | Assiut University     | 55555555-5555-5555-5555-555555555555 |
        /// | Benha University      | 66666666-6666-6666-6666-666666666666 |
        ///
        /// Filter by role using the `name` or `department` query parameters.
        /// </remarks>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<UniversityMemberDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<UniversityMemberDto>>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 12,
            [FromQuery] string? name = null,
            [FromQuery] string? department = null)
        {
            var result = await _mediator.Send(new GetAllUniversityMembersQuery(page, pageSize, name, department));
            return HandleResult(result);
        }

        /// <summary>
        /// Get all university members by UniversityId.
        /// </summary>
        /// <remarks>
        /// Available UniversityIds:
        ///
        ///     11111111-1111-1111-1111-111111111111  → Cairo University
        ///     22222222-2222-2222-2222-222222222222  → Ain Shams University
        ///     33333333-3333-3333-3333-333333333333  → Mansoura University
        ///     44444444-4444-4444-4444-444444444444  → Alexandria University
        ///     55555555-5555-5555-5555-555555555555  → Assiut University
        ///     66666666-6666-6666-6666-666666666666  → Benha University
        /// </remarks>
        [HttpGet("{universityId}")]
        [ProducesResponseType(typeof(ApiResponse<List<UniversityMemberDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<List<UniversityMemberDto>>>> GetByUniversityId(Guid universityId)
        {
            var result = await _mediator.Send(new GetUniversityMemberByUniversityIdQuery(universityId));
            return HandleResult(result);
        }
    }
}
