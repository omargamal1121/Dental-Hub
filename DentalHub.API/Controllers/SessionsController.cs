using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MediatR;
using DentalHub.Application.Commands.Sessions;
using DentalHub.Application.Queries.Sessions;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Common;
using Microsoft.AspNetCore.Authorization;

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

        // ──────────────────────────────────────────
        //  CRUD
        // ──────────────────────────────────────────

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

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteSessionCommand(id));
            return HandleResult(result);
        }

        [HttpPatch("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(Guid id, [FromBody] UpdateSessionStatusCommand command)
        {
            if (id != command.SessionId)
                return CreateErrorResponse<bool>("Id mismatch", 400);

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
                return CreateErrorResponse<Guid>("Id mismatch", 400);

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

        // ──────────────────────────────────────────
        //  Filter endpoints (by entity)
        // ──────────────────────────────────────────

        [HttpGet("student/{studentId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SessionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<SessionDto>>>> GetByStudent(
            Guid studentId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetSessionsByStudentIdQuery(studentId, page, pageSize));
            return HandleResult(result);
        }

        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SessionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<SessionDto>>>> GetByPatient(
            Guid patientId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetSessionsByPatientIdQuery(patientId, page, pageSize));
            return HandleResult(result);
        }

        [HttpGet("case/{caseId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SessionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<SessionDto>>>> GetByCase(
            Guid caseId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetSessionsByCaseIdQuery(caseId, page, pageSize));
            return HandleResult(result);
        }

        // ──────────────────────────────────────────
        //  Schedule endpoints  (NEW)
        // ──────────────────────────────────────────

        /// <summary>
        /// GET /api/sessions/schedule
        /// All sessions (paginated). Optionally filter by status.
        /// </summary>
        [HttpGet("schedule")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SessionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<SessionDto>>>> GetAllSchedule(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? status = null)
        {
            var result = await _mediator.Send(new GetAllSessionsQuery(page, pageSize, status));
            return HandleResult(result);
        }

        /// <summary>
        /// GET /api/sessions/schedule/upcoming
        /// Upcoming sessions only (status = Scheduled, StartAt > now).
        /// Optional query params: studentId, patientId to narrow results.
        /// </summary>
        [HttpGet("schedule/upcoming")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SessionDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<SessionDto>>>> GetUpcoming(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? studentId = null,
            [FromQuery] Guid? patientId = null)
        {
            var result = await _mediator.Send(
                new GetUpcomingSessionsQuery(page, pageSize, studentId, patientId));
            return HandleResult(result);
        }
    }
}
