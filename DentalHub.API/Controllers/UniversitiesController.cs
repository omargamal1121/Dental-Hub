using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.Universities;
using DentalHub.Application.Queries.Universities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UniversitiesController : BaseController
    {
        private readonly IMediator _mediator;

        public UniversitiesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("lookup")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<UniversityLookupDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<UniversityLookupDto>>>> GetLookup()
        {
            var result = await _mediator.Send(new GetAllUniversitiesQuery());
            return HandleResult(result);
        }
    }
}
