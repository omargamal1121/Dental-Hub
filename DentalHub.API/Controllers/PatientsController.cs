using Microsoft.AspNetCore.Mvc;
using MediatR;
using DentalHub.Application.Commands.Patient;
using DentalHub.Application.Queries.Patient;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.Patients;

namespace DentalHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : BaseController
    {
        private readonly IMediator _mediator;

        public PatientsController(IMediator mediator) : base()
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> Create([FromBody] CreatePatientCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeletePatientCommand(id));
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<PatientDto>>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetPatientByIdQuery(id));
            return HandleResult(result);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<PatientDto>>>> GetAll()
        {
            var result = await _mediator.Send(new GetAllPatientsQuery());
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid id, [FromBody] UpdatePatientCommand command)
        {
             if (id != command.UserId)
            {
                return CreateErrorResponse<bool>("Id mismatch", 400); 
            }
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
    }
}
