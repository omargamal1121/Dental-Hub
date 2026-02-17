using DentalHub.Application.Commands.CaseRequests;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.Queries.CaseRequests;
using DentalHub.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CaseRequestsController : BaseController
    {
        private readonly IMediator _mediator;

        public CaseRequestsController(IMediator mediator) : base()
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CaseRequestDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<CaseRequestDto>>> Create([FromBody] CreateCaseRequestDto dto)
        {
            var command = new CreateCaseRequestCommand(dto.PatientCasePublicId, dto.StudentPublicId, dto.DoctorPublicId, dto.Description);
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CaseRequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<CaseRequestDto>>> GetById(string id)
        {
            var result = await _mediator.Send(new GetCaseRequestByIdQuery(id));
            return HandleResult(result);
        }

        [HttpGet("doctor/{doctorId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CaseRequestDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<CaseRequestDto>>>> GetRequestsByDoctor(string doctorId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetCaseRequestsByDoctorIdQuery(doctorId, page, pageSize));
            return HandleResult(result);
        }

        [HttpGet("student/{studentId}")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CaseRequestDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<CaseRequestDto>>>> GetRequestsByStudent(string studentId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _mediator.Send(new GetCaseRequestsByStudentIdQuery(studentId, page, pageSize));
            return HandleResult(result);
        }

        [HttpPost("approve")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Approve([FromBody] ApproveCaseRequestDto dto)
        {
            var result = await _mediator.Send(new ApproveCaseRequestCommand(dto.RequestId, dto.DoctorId));
            return HandleResult(result);
        }

        [HttpPost("reject/{id}/{doctorId}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Reject(string id, string doctorId)
        {
            var result = await _mediator.Send(new RejectCaseRequestCommand(id, doctorId));
            return HandleResult(result);
        }

        [HttpDelete("{requestId}/{studentId}")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<object>>> Cancel(string requestId, string studentId)
        {
            var result = await _mediator.Send(new CancelCaseRequestCommand(requestId, studentId));
            return HandleResult(result);
        }

        [HttpPost("case/{caseId}/reject-all")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> RejectAllForCase(string caseId)
        {
            var result = await _mediator.Send(new RejectAllRequestsForCaseCommand(caseId));
            return HandleResult(result);
        }

        [HttpPost("case/{caseId}/mark-taken/{approvedRequestId}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> MarkTakenForCase(string caseId, string approvedRequestId)
        {
            var result = await _mediator.Send(new MarkAllRequestsTakenForCaseCommand(caseId, approvedRequestId));
            return HandleResult(result);
        }

        [HttpGet("case/{caseId}")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CaseRequestDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CaseRequestDto>>>> GetByCase(string caseId, [FromQuery] RequestStatus? status = null)
        {
            var result = await _mediator.Send(new GetCaseRequestsByCaseIdQuery(caseId, status));
            return HandleResult(result);
        }

        [HttpPost("student/{studentId}/cancel-all")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<bool>>> CancelAllForStudent(string studentId)
        {
            var result = await _mediator.Send(new CancelAllStudentRequestsCommand(studentId));
            return HandleResult(result);
        }
    }
}
