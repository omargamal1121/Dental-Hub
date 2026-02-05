using DentalHub.Application.Commands.Students;
using DentalHub.Application.Queries.Students;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.Students;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Services.Students;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<ApiResponse<Guid>>> Create([FromBody] CreateStudentCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<StudentDto>>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetStudentByIdQuery(id));
            return HandleResult(result);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<StudentDto>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetAllStudentsQuery(page, pageSize));
            return HandleResult(result);
        }

        [HttpPut("{id}")]
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
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteStudentCommand(id));
            return HandleResult(result);
        }

        [HttpGet("{id}/available-cases")]
        public async Task<ActionResult<ApiResponse<List<PatientCaseDto>>>> GetAvailableCases(Guid id, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetAvailableCasesForStudentQuery(id, page, pageSize));
            return HandleResult(result);
        }

        [HttpGet("{id}/statistics")]
        public async Task<ActionResult<ApiResponse<StudentStatsDto>>> GetStatistics(Guid id)
        {
            var result = await _mediator.Send(new GetStudentStatisticsQuery(id));
            return HandleResult(result);
        }
    }
}
