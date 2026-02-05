using Microsoft.AspNetCore.Mvc;
using MediatR;
using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Queries.PatientCase;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.Cases;

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
        public async Task<ActionResult<ApiResponse<Guid>>> Create([FromBody] CreatePatientCaseCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeletePatientCaseCommand(id));
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PatientCaseDto>>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetPatientCaseByIdQuery(id));
            return HandleResult(result);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<ApiResponse<List<PatientCaseDto>>>> GetByPatientId(Guid patientId)
        {
            var result = await _mediator.Send(new GetPatientCasesByPatientIdQuery(patientId));
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid id, [FromBody] UpdatePatientCaseCommand command)
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
