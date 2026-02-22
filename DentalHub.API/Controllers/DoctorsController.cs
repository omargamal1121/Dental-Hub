using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using DentalHub.Application.Commands.Doctor;
using DentalHub.Application.Queries.Doctor;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.Doctors;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Doctors;
using DentalHub.Application.Services.Cases;

namespace DentalHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DoctorsController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IDoctorService _doctorService;
        private readonly ICaseRequestService _caseRequestService;

        public DoctorsController(
            IMediator mediator,
            IDoctorService doctorService,
            ICaseRequestService caseRequestService) : base()
        {
            _mediator = mediator;
            _doctorService = doctorService;
            _caseRequestService = caseRequestService;
        }

        #region Admin Endpoints (Public)

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<string>>> Create([FromBody] CreateDoctorCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(string id)
        {
            var result = await _mediator.Send(new DeleteDoctorCommand(id));
            return HandleResult(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<DoctorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<DoctorDto>>> GetById(string id)
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
        public async Task<ActionResult<ApiResponse<bool>>> Update(string id, [FromBody] UpdateDoctorCommand command)
        {
            if (id != command.PublicId)
            {
                return CreateErrorResponse<bool>("Id mismatch", 400);
            }
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        #endregion

        #region Doctor-Specific Endpoints (JWT Auth Required)

        /// Get my requests as a doctor (from JWT token)
        [Authorize(Roles = "Doctor")]
        [HttpGet("my-requests")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CaseRequestDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<PagedResult<CaseRequestDto>>>> GetMyRequests(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var doctorId = GetUserIdFromToken();

            if (doctorId == null)
            {
                return CreateErrorResponse<PagedResult<CaseRequestDto>>(
                    "Unauthorized: Invalid token",
                    StatusCodes.Status401Unauthorized);
            }

            // ✅ إصلاح: ToString() + page + pageSize
            var result = await _caseRequestService.GetRequestsByDoctorIdAsync(
                doctorId.Value.ToString(), page, pageSize);

            if (!result.IsSuccess)
            {
                return CreateErrorResponse<PagedResult<CaseRequestDto>>(
                    result.Message ?? "Failed to retrieve requests",
                    StatusCodes.Status400BadRequest,
                    result.Errors);
            }

            return Ok(ApiResponse<PagedResult<CaseRequestDto>>.CreateSuccessResponse(
                "Requests retrieved successfully",
                result.Data!,
                StatusCodes.Status200OK));
        }

        /// Get my profile as a doctor (from JWT token)
        [Authorize(Roles = "Doctor")]
        [HttpGet("my-profile")]
        [ProducesResponseType(typeof(ApiResponse<DoctorDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<DoctorDto>>> GetMyProfile()
        {
            var userId = GetUserIdFromToken();

            if (userId == null)
            {
                return CreateErrorResponse<DoctorDto>(
                    "Unauthorized: Invalid token",
                    StatusCodes.Status401Unauthorized);
            }

            var result = await _doctorService.GetDoctorByIdAsync(userId.Value.ToString());

            if (!result.IsSuccess)
            {
                return CreateErrorResponse<DoctorDto>(
                    result.Message ?? "Doctor profile not found",
                    StatusCodes.Status404NotFound,
                    result.Errors);
            }

            return Ok(ApiResponse<DoctorDto>.CreateSuccessResponse(
                "Profile retrieved successfully",
                result.Data!,
                StatusCodes.Status200OK));
        }

        /// Approve a case request (with authorization check)
        [Authorize(Roles = "Doctor")]
        [HttpPost("requests/{requestId}/approve")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> ApproveRequest(
            Guid requestId,
            [FromBody] ApproveRequestBody body)
        {
            var doctorIdFromToken = GetUserIdFromToken();

            if (doctorIdFromToken == null)
            {
                return CreateErrorResponse<bool>(
                    "Unauthorized: Invalid token",
                    StatusCodes.Status401Unauthorized);
            }

            var dto = new ApproveCaseRequestDto
            {
                RequestId = requestId.ToString(),
                DoctorId = doctorIdFromToken.Value.ToString(),
                IsApproved = true
            };

            var result = await _caseRequestService.ApproveOrRejectRequestAsync(requestId, dto);

            if (!result.IsSuccess)
            {
                var statusCode = result.Message?.Contains("not authorized") == true
                    ? StatusCodes.Status403Forbidden
                    : StatusCodes.Status400BadRequest;

                return CreateErrorResponse<bool>(
                    result.Message ?? "Failed to approve request",
                    statusCode,
                    result.Errors);
            }

            return Ok(ApiResponse<bool>.CreateSuccessResponse(
                "Request approved successfully",
                result.Data,
                StatusCodes.Status200OK));
        }

        /// Reject a case request (with authorization check)
        [Authorize(Roles = "Doctor")]
        [HttpPost("requests/{requestId}/reject")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> RejectRequest(
            Guid requestId,
            [FromBody] RejectRequestBody body)
        {
            var doctorIdFromToken = GetUserIdFromToken();

            if (doctorIdFromToken == null)
            {
                return CreateErrorResponse<bool>(
                    "Unauthorized: Invalid token",
                    StatusCodes.Status401Unauthorized);
            }

            var dto = new ApproveCaseRequestDto
            {
                RequestId = requestId.ToString(),
                DoctorId = doctorIdFromToken.Value.ToString(),
                IsApproved = false,
                RejectionReason = body.RejectionReason
            };

            var result = await _caseRequestService.ApproveOrRejectRequestAsync(requestId, dto);

            if (!result.IsSuccess)
            {
                var statusCode = result.Message?.Contains("not authorized") == true
                    ? StatusCodes.Status403Forbidden
                    : StatusCodes.Status400BadRequest;

                return CreateErrorResponse<bool>(
                    result.Message ?? "Failed to reject request",
                    statusCode,
                    result.Errors);
            }

            return Ok(ApiResponse<bool>.CreateSuccessResponse(
                "Request rejected successfully",
                result.Data,
                StatusCodes.Status200OK));
        }

        /// Get my statistics as a doctor
        [Authorize(Roles = "Doctor")]
        [HttpGet("my-statistics")]
        [ProducesResponseType(typeof(ApiResponse<DoctorStatsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<DoctorStatsDto>>> GetMyStatistics()
        {
            var doctorId = GetUserIdFromToken();

            if (doctorId == null)
            {
                return CreateErrorResponse<DoctorStatsDto>(
                    "Unauthorized: Invalid token",
                    StatusCodes.Status401Unauthorized);
            }

            var result = await _doctorService.GetDoctorStatisticsAsync(doctorId.Value.ToString());

            if (!result.IsSuccess)
            {
                return CreateErrorResponse<DoctorStatsDto>(
                    result.Message ?? "Failed to retrieve statistics",
                    StatusCodes.Status400BadRequest,
                    result.Errors);
            }

            return Ok(ApiResponse<DoctorStatsDto>.CreateSuccessResponse(
                "Statistics retrieved successfully",
                result.Data!,
                StatusCodes.Status200OK));
        }

        #endregion
    }

    #region Request Body DTOs

    public class ApproveRequestBody
    {
    }

    public class RejectRequestBody
    {
        public string? RejectionReason { get; set; }
    }

    #endregion
}
