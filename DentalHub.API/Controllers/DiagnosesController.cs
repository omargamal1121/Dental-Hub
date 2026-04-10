using DentalHub.Application.Commands.Diagnoses;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Diagnoses;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.Queries.Diagnoses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DentalHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosesController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;

        public DiagnosesController(IMediator mediator, IConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<DiagnosisDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<DiagnosisDto>>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetDiagnosisByIdQuery(id));
            return HandleResult(result);
        }

        [HttpGet("case/{patientCaseId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<DiagnosisDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<DiagnosisDto>>>> GetByCaseId(Guid patientCaseId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetDiagnosesByPatientCaseIdQuery(patientCaseId, page, pageSize));
            return HandleResult(result);
        }

        [HttpPost]
        [Authorize(Roles = "Student,Doctor,Admin")]
        [ProducesResponseType(typeof(ApiResponse<DiagnosisDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<DiagnosisDto>>> Create([FromBody] CreateDiagnosisCommand command)
        {
            var userId = GetUserIdFromToken();
            var role = GetUserRoleFromToken();

            if (userId == null || string.IsNullOrEmpty(role))
            {
                return CreateErrorResponse<DiagnosisDto>("Unauthorized", StatusCodes.Status401Unauthorized);
            }

            command.CreatedById = userId;
            command.Role = role;

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpPost("ai/create")]
        [ProducesResponseType(typeof(ApiResponse<DiagnosisDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<DiagnosisDto>>> CreateByAI([FromBody] CreateDiagnosisCommand command)
        {
            var headerApiKey = Request.Headers["X-AI-API-KEY"].FirstOrDefault();
            var configuredApiKey = _configuration["AI_Configuration:ApiKey"];

            if (string.IsNullOrEmpty(headerApiKey) || headerApiKey != configuredApiKey)
            {
                return CreateErrorResponse<DiagnosisDto>("Invalid or missing AI API Key.", StatusCodes.Status401Unauthorized);
            }

         
            command.Stage = Domain.Entities.DiagnosisStage.AI;
            command.Role = "AI";
            command.CreatedById=null;
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        [ProducesResponseType(typeof(ApiResponse<DiagnosisDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<DiagnosisDto>>> Update(Guid id, [FromBody] UpdateDiagnosisCommand command)
        {
            if (id != command.Id)
            {
                return CreateErrorResponse<DiagnosisDto>("Id mismatch", 400);
            }
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Doctor,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
        {
            var result = await _mediator.Send(new DeleteDiagnosisCommand(id));
            return HandleResult(result);
        }

        [HttpPost("{id}/accept")]
        [Authorize(Roles = "Doctor,Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> Accept(Guid id)
        {
            var result = await _mediator.Send(new AcceptDiagnosisCommand(id));
            return HandleResult(result);
        }
    }
}
