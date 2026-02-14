using DentalHub.Application.Commands.CaseTypes;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.CaseTypes;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.Queries.CaseTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace DentalHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaseTypesController : BaseController
    {
        private readonly IMediator _mediator;

        public CaseTypesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CaseTypeDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<CaseTypeDto>>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await _mediator.Send(new GetAllCaseTypesQuery(page, pageSize, search));
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CaseTypeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CaseTypeDto>>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetCaseTypeByIdQuery(id));
            return HandleResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CaseTypeDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CaseTypeDto>>> Create([FromBody] CreateCaseTypeCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CaseTypeDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CaseTypeDto>>> Update(Guid id, [FromBody] UpdateCaseTypeCommand command)
        {
            if (id != command.Id)
            {
                return CreateErrorResponse<CaseTypeDto>("Id mismatch", 400);
            }
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCaseTypeCommand(id));
            return HandleResult(result);
        }
    }
}
