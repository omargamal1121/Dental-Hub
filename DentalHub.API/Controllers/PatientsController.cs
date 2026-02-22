using DentalHub.Application.Commands.Patient;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Patients;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.Queries.Patient;
using DentalHub.Application.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalHub.API.Controllers
{
    [Authorize]
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
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<string>>> Create([FromBody] CreatePatientCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(string id)
        {
            var result = await _mediator.Send(new DeletePatientCommand(id));
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PatientDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PatientDto>>> GetById(string id)
        {
            var result = await _mediator.Send(new GetPatientByIdQuery(id));
            return HandleResult(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PatientDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<PatientDto>>>> GetAll(
            [FromQuery] FilterPatientDto filter,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetAllPatientsQuery(filter, pageNumber, pageSize));
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Update(string id, [FromBody] UpdatePatientCommand command)
        {
             if (id != command.PublicId)
            {
                return CreateErrorResponse<bool>("Id mismatch", 400); 
            }
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
    }
}
