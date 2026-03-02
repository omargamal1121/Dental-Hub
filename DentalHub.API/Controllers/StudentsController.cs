using DentalHub.Application.Commands.Students;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.DTOs.Identity;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.Students;
using DentalHub.Application.Queries.CaseRequests;
using DentalHub.Application.Queries.Students;
using DentalHub.Application.Services.Students;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<string>>> Create([FromBody] RegisterStudentDto  registerStudent)
        {
            var command = new CreateStudentCommand(registerStudent.FullName, registerStudent.Email, registerStudent.Password, registerStudent.UniversityId,
                registerStudent.Level, registerStudent.Username, registerStudent.Phone);
         
			var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<StudentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<StudentDto>>> GetById(string id)
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
        public async Task<ActionResult<ApiResponse<bool>>> Update(string id, [FromBody] UpdateStudentCommand command)
        {
            if (id != command.PublicId)
            {
                return CreateErrorResponse<bool>("Id mismatch", 400);
            }
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(string id)
        {
            var result = await _mediator.Send(new DeleteStudentCommand(id));
            return HandleResult(result);
        }

       
        [HttpGet("my-cases")]
        [Authorize(Roles = "Student")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<PatientCaseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PagedResult<PatientCaseDto>>>> GetMyCases(
            [FromQuery] string? caseType = null,
            [FromQuery] int     page     = 1,
            [FromQuery] int     pageSize = 10)
        {
            var studentPublicId = GetUserIdFromToken();
            if (string.IsNullOrEmpty(studentPublicId))
                return CreateErrorResponse<PagedResult<PatientCaseDto>>("Unauthorized", 401);

            var result = await _mediator.Send(new GetMyCasesForStudentQuery(studentPublicId, caseType, page, pageSize));
            return HandleResult(result);
        }

        [HttpGet("available-cases")]
        [Authorize(Roles = "Student")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<AvailableCasesDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PagedResult<AvailableCasesDto>>>> GetAvailableCases(
            [FromQuery] string? caseType = null,
            [FromQuery] int     page     = 1,
            [FromQuery] int     pageSize = 10)
        {
            var studentPublicId = GetUserIdFromToken();
            if (string.IsNullOrEmpty(studentPublicId))
                return CreateErrorResponse<PagedResult<AvailableCasesDto>>("Unauthorized", 401);

            var result = await _mediator.Send(new GetAvailableCasesForStudentQuery(studentPublicId, caseType, page, pageSize));
            return HandleResult(result);
        }

        [HttpGet("my-requests")]
        [Authorize(Roles = "Student")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CaseRequestDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PagedResult<CaseRequestDto>>>> GetMyRequests(
            [FromQuery] string? status = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var studentPublicId = GetUserIdFromToken();
            if (string.IsNullOrEmpty(studentPublicId))
                return CreateErrorResponse<PagedResult<CaseRequestDto>>("Unauthorized", 401);

            RequestStatus? requestStatus = null;
            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<RequestStatus>(status, true, out var parsedStatus))
                {
                    requestStatus = parsedStatus;
                }
                else
                {
                    return CreateErrorResponse<PagedResult<CaseRequestDto>>($"Invalid status: {status}", 400);
                }
            }

            var result = await _mediator.Send(new GetCaseRequestsByStudentIdQuery(studentPublicId, requestStatus, page, pageSize));
            return HandleResult(result);
        }

        [HttpGet("{id}/statistics")]
        [ProducesResponseType(typeof(ApiResponse<StudentStatsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<StudentStatsDto>>> GetStatistics(string id)
        {
            var result = await _mediator.Send(new GetStudentStatisticsQuery(id));
            return HandleResult(result);
        }
    }
}
