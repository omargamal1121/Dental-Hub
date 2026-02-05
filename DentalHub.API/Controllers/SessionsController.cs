using Microsoft.AspNetCore.Mvc;
using MediatR;
using DentalHub.Application.Commands.Sessions;
using DentalHub.Application.Queries.Sessions;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.Sessions;

namespace DentalHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SessionsController : BaseController
    {
        private readonly IMediator _mediator;

        public SessionsController(IMediator mediator) : base()
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> Create([FromBody] CreateSessionCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<SessionDto>>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetSessionByIdQuery(id));
            return HandleResult(result);
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<SessionDto>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetAllSessionsQuery(page, pageSize));
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteSessionCommand(id));
            return HandleResult(result);
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<ApiResponse<List<SessionDto>>>> GetByStudent(Guid studentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetSessionsByStudentIdQuery(studentId, page, pageSize));
            return HandleResult(result);
        }

        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<ApiResponse<List<SessionDto>>>> GetByPatient(Guid patientId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetSessionsByPatientIdQuery(patientId, page, pageSize));
            return HandleResult(result);
        }

        [HttpGet("case/{caseId}")]
        public async Task<ActionResult<ApiResponse<List<SessionDto>>>> GetByCase(Guid caseId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetSessionsByCaseIdQuery(caseId, page, pageSize));
            return HandleResult(result);
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(Guid id, [FromBody] UpdateSessionStatusCommand command)
        {
            if (id != command.SessionId)
            {
                 return CreateErrorResponse<bool>("Id mismatch", 400);
            }
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpPost("{id}/notes")]
        public async Task<ActionResult<ApiResponse<Guid>>> AddNote(Guid id, [FromBody] AddSessionNoteCommand command)
        {
             if (id != command.SessionId)
            {
                 return CreateErrorResponse<Guid>("Id mismatch", 400);
            }
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpGet("{id}/notes")]
        public async Task<ActionResult<ApiResponse<List<SessionNoteDto>>>> GetNotes(Guid id)
        {
            var result = await _mediator.Send(new GetSessionNotesQuery(id));
            return HandleResult(result);
        }
    }
}
