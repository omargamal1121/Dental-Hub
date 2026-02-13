using DentalHub.Application.Commands.CaseTypes;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.CaseTypes;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.Queries.CaseTypes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<ApiResponse<List<CaseTypeDto>>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null)
        {
            var result = await _mediator.Send(new GetAllCaseTypesQuery(page, pageSize, search));
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CaseTypeDto>>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetCaseTypeByIdQuery(id));
            return HandleResult(result);
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CaseTypeDto>>> Create([FromBody] CreateCaseTypeCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
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
        public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteCaseTypeCommand(id));
            return HandleResult(result);
        }
    }
}
