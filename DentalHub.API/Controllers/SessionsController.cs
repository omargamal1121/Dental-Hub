using DentalHub.Application.Commands.Sessions;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.Queries.Sessions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Claims;

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
        public async Task<ActionResult<ApiResponse<bool>>> Create([FromBody] CreateSessionCommand command)
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
        [ProducesResponseType(typeof(ApiResponse<SessionNoteDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<SessionNoteDto>>> AddNote(Guid id, [FromBody] AddSessionNoteCommand command)
        {
            if (id != command.SessionId)
                return CreateErrorResponse<SessionNoteDto>("Id mismatch", 400);

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

        [HttpPost("{id}/notes/{noteId}/media")]
        [ProducesResponseType(typeof(ApiResponse<SessionMediaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<SessionMediaDto>>> UploadNoteMedia(Guid id, Guid noteId, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return CreateErrorResponse<SessionMediaDto>("File is required", 400);

            var result = await _mediator.Send(new AddNoteMediaCommand(id, noteId, file));
            return HandleResult(result);
        }

        [HttpGet("{id}/notes/{noteId}/media")]
        [ProducesResponseType(typeof(ApiResponse<List<SessionMediaDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<List<SessionMediaDto>>>> GetNoteMedia(Guid id, Guid noteId)
        {
            var result = await _mediator.Send(new GetNoteMediaQuery(id, noteId));
            return HandleResult(result);
        }


        // ──────────────────────────────────────────
        //  Media
        // ──────────────────────────────────────────

        [HttpPost("{id}/media")]
        [ProducesResponseType(typeof(ApiResponse<SessionMediaDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<SessionMediaDto>>> UploadMedia(Guid id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return CreateErrorResponse<SessionMediaDto>("File is required", 400);

            var result = await _mediator.Send(new AddSessionMediaCommand(id, file));
            return HandleResult(result);
        }

        [HttpGet("{id}/media")]
        [ProducesResponseType(typeof(ApiResponse<List<SessionMediaDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<List<SessionMediaDto>>>> GetMedia(Guid id)
        {
            var result = await _mediator.Send(new GetSessionMediaQuery(id));
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

        // ──────────────────────────────────────────
        //  Evaluation
        // ──────────────────────────────────────────

        [HttpPost("{id}/evaluate")]
        [Authorize(Roles = "Doctor")]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<Guid>>> Evaluate(Guid id, [FromBody] EvaluateRequest request)
        {
            var doctorIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(doctorIdClaim))
                return CreateErrorResponse<Guid>("Unauthorized: Doctor ID not found in token", 401);

            var doctorId = Guid.Parse(doctorIdClaim);

            var result = await _mediator.Send(new EvaluateSessionCommand(id, doctorId, request.Grade, request.Note, request.IsFinalSession));

            return HandleResult(result);
        }
    }
}
