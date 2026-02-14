using DentalHub.Application.Commands.Students;
using DentalHub.Application.Queries.Students;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.Students;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Services.Students;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using DentalHub.Application.Common;

namespace DentalHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : BaseController
    {
        private readonly IMediator _mediator;

        public StudentsController(IMediator mediator) : base()
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Guid>>> Create([FromBody] CreateStudentCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<StudentDto>>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetStudentByIdQuery(id));
            return HandleResult(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<StudentDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<StudentDto>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetAllStudentsQuery(page, pageSize));
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid id, [FromBody] UpdateStudentCommand command)
        {
            if (id != command.UserId)
            {
                return CreateErrorResponse<bool>("Id mismatch", 400);
            }
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteStudentCommand(id));
            return HandleResult(result);
        }

        [HttpGet("{id}/available-cases")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PatientCaseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PagedResult<PatientCaseDto>>>> GetAvailableCases(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetAvailableCasesForStudentQuery(id, page, pageSize));
            return HandleResult(result);
        }

        [HttpGet("{id}/statistics")]
        [ProducesResponseType(typeof(ApiResponse<StudentStatsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<StudentStatsDto>>> GetStatistics(Guid id)
        {
            var result = await _mediator.Send(new GetStudentStatisticsQuery(id));
            return HandleResult(result);
        }
    }
}
