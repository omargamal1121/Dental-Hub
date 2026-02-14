using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MediatR;
using DentalHub.Application.Commands.Sessions;
using DentalHub.Application.Queries.Sessions;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Common;

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
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Guid>>> Create([FromBody] CreateSessionCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SessionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<SessionDto>>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetSessionByIdQuery(id));
            return HandleResult(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SessionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<SessionDto>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetAllSessionsQuery(page, pageSize));
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteSessionCommand(id));
            return HandleResult(result);
        }

        [HttpGet("student/{studentId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SessionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<SessionDto>>>> GetByStudent(Guid studentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetSessionsByStudentIdQuery(studentId, page, pageSize));
            return HandleResult(result);
        }

        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SessionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<SessionDto>>>> GetByPatient(Guid patientId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetSessionsByPatientIdQuery(patientId, page, pageSize));
            return HandleResult(result);
        }

        [HttpGet("case/{caseId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SessionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<SessionDto>>>> GetByCase(Guid caseId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetSessionsByCaseIdQuery(caseId, page, pageSize));
            return HandleResult(result);
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
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
        [ProducesResponseType(typeof(ApiResponse<List<SessionNoteDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<List<SessionNoteDto>>>> GetNotes(Guid id)
        {
            var result = await _mediator.Send(new GetSessionNotesQuery(id));
            return HandleResult(result);
        }
    }
}
