using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using MediatR;
using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Queries.PatientCase;
using DentalHub.Application.Queries.Students;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Common;
using Microsoft.AspNetCore.Authorization;

namespace DentalHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CasesController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public CasesController(IMediator mediator, IConfiguration configuration) : base()
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        /// <summary>
        /// Get all patient cases with optional search (patient name / case-type name),
        /// optional status filter, and pagination.
        /// </summary>
        /// <param name="search">Partial match on patient full-name or case-type name.</param>
        /// <param name="status">Filter by status: Pending | InProgress | Completed | Cancelled | UnderReview</param>
        /// <param name="page">Page number (default 1).</param>
        /// <param name="pageSize">Items per page (default 10).</param>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PatientCaseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<PagedResult<PatientCaseDto>>>> GetAll([FromQuery] CaseFilterDto filter)
        {
            var result = await _mediator.Send(new GetAllCasesQuery(filter));
            return HandleResult(result);
        }




        [HttpPost]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Guid>>> Create([FromForm] CreatePatientCaseCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpPost("ai/create")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateByAI([FromForm] CreatePatientCaseCommand command)
        {
            var headerApiKey = Request.Headers["X-AI-API-KEY"].FirstOrDefault();
            var configuredApiKey = _configuration["AI_Configuration:ApiKey"];

            if (string.IsNullOrEmpty(headerApiKey) || headerApiKey != configuredApiKey)
            {
                return CreateErrorResponse<Guid>("Invalid or missing AI API Key.", StatusCodes.Status401Unauthorized);
            }

            var aiCommand = command with { CreatedByRole = "AI", CreatedById = null };
            var result = await _mediator.Send(aiCommand);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeletePatientCaseCommand(id));
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PatientCaseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PatientCaseDto>>> GetById(Guid id)
        {
            // Pass user context so the service can compute flags and available actions.
            // Both values are nullable – anonymous callers get no flags (all false).
            var userId = GetUserIdFromToken();
            var userRole = GetUserRoleFromToken();

            var result = await _mediator.Send(new GetPatientCaseByIdQuery(id, userId, userRole));
            return HandleResult(result);
        }

        [HttpGet("patient/{patientId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PatientCaseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<PagedResult<PatientCaseDto>>>> GetByPatientId(Guid patientId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetPatientCasesByPatientIdQuery(patientId, page, pageSize));
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid id, [FromBody] UpdatePatientCaseCommand command)
        {
            if (id != command.Id)
            {
                return CreateErrorResponse<bool>("Id mismatch", 400);
            }
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> UpdateStatus(Guid id, [FromBody] UpdateCaseStatusDto dto)
        {
            var result = await _mediator.Send(new UpdatePatientCaseStatusCommand(id, dto.Status));
            return HandleResult(result);
        }

        [HttpPut("{id}/assign-university")]
        [Authorize(Roles = "Student,Doctor,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> AssignUniversity(Guid id, [FromBody] AssignCaseUniversityDto dto)
        {
            var userId = GetUserIdFromToken();
            var role = GetUserRoleFromToken();

            if (userId == null || string.IsNullOrEmpty(role))
            {
                return CreateErrorResponse<object>("Unauthorized", StatusCodes.Status401Unauthorized);
            }

            var command = new AssignCaseUniversityCommand(id, dto.UniversityId, dto.IsPublic, userId.Value, role);
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }
    }
}
