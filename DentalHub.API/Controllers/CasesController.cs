using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MediatR;
using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Queries.PatientCase;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Common;

namespace DentalHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CasesController : BaseController
    {
        private readonly IMediator _mediator;

        public CasesController(IMediator mediator) : base()
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<string>>> Create([FromBody] CreatePatientCaseCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(string id)
        {
            var result = await _mediator.Send(new DeletePatientCaseCommand(id));
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PatientCaseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PatientCaseDto>>> GetById(string id)
        {
            var result = await _mediator.Send(new GetPatientCaseByIdQuery(id));
            return HandleResult(result);
        }

        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PatientCaseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PagedResult<PatientCaseDto>>>> GetByPatientId(string patientId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetPatientCasesByPatientIdQuery(patientId, page, pageSize));
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Update(string     id, [FromBody] UpdatePatientCaseCommand command)
        {
             if (id != command.Id)
            {
                return CreateErrorResponse<bool>("Id mismatch", 400);
            }
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
    }
}
