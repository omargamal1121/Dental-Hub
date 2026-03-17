using DentalHub.Application.Commands.Doctor;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.DTOs.Doctors;
using DentalHub.Application.DTOs.Identity;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.Queries.Doctor;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DentalHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : BaseController
    {
        private readonly IMediator _mediator;

        public DoctorsController(IMediator mediator) : base()
        {
            _mediator = mediator;
        }

        #region Admin Endpoints

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<Guid>>> Create([FromBody] RegisterDoctorDto registerDoctor)
        {
            var registerCommand = new CreateDoctorCommand(registerDoctor.FullName
                ,registerDoctor.Email
                ,registerDoctor.Password,
                registerDoctor.Specialty,
                registerDoctor.UniversityId,
                registerDoctor.Username,
                registerDoctor.Phone);
			var result = await _mediator.Send(registerCommand);
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<DoctorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<DoctorDto>>> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetDoctorByIdQuery(id));
            return HandleResult(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<DoctorlistDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<PagedResult<DoctorlistDto>>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? name = null,
            [FromQuery] string? spec = null)
        {
            var result = await _mediator.Send(new GetAllDoctorsQuery(page, pageSize, name, spec));
            return HandleResult(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Update(Guid id, [FromBody] UpdateDoctorCommand command)
        {
            if (id != command.PublicId)
                return CreateErrorResponse<bool>("Id mismatch", 400);

            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        #endregion

        #region Doctor-Specific Endpoints (JWT Auth Required)

        /// Get my requests as a doctor (DoctorId from JWT token)
        [Authorize(Roles = "Doctor")]
        [HttpGet("my-requests")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CaseRequestDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PagedResult<CaseRequestDto>>>> GetMyRequests(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var doctorId = GetUserIdFromToken();
            if (doctorId == null)
                return CreateErrorResponse<PagedResult<CaseRequestDto>>("Unauthorized: Invalid token", 401);

            var result = await _mediator.Send(new GetMyRequestsQuery(doctorId.Value, page, pageSize));
            return HandleResult(result);
        }

        /// Approve a case request (DoctorId from JWT token)
        [Authorize(Roles = "Doctor")]
        [HttpPost("requests/{requestId}/approve")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> ApproveRequest(Guid requestId)
        {
            var doctorId = GetUserIdFromToken();
            if (doctorId == null)
                return CreateErrorResponse<bool>("Unauthorized: Invalid token", 401);

            var result = await _mediator.Send(new ApproveMyRequestCommand(requestId, doctorId.Value));
            return HandleResult(result);
        }

        /// Reject a case request (DoctorId from JWT token)
        [Authorize(Roles = "Doctor")]
        [HttpPost("requests/{requestId}/reject")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> RejectRequest(Guid requestId)
        {
            var doctorId = GetUserIdFromToken();
            if (doctorId == null)
                return CreateErrorResponse<bool>("Unauthorized: Invalid token", 401);

            var result = await _mediator.Send(new RejectMyRequestCommand(requestId, doctorId.Value));
            return HandleResult(result);
        }

        #endregion
    }
}
