using DentalHub.Application.Commands.Auth;
using DentalHub.Application.DTOs.Auth;
using DentalHub.Application.DTOs.Doctors;
using DentalHub.Application.DTOs.Students;
using DentalHub.Application.DTOs.Patients;
using DentalHub.Application.DTOs.Admins;
using DentalHub.Application.Common;
using DentalHub.Application.Queries.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using DentalHub.Application.DTOs.Shared;
using Swashbuckle.AspNetCore.Annotations;

namespace DentalHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) : base()
        {
            _mediator = mediator;
        }

        [HttpPost("Login")]
        [ProducesResponseType(typeof(ApiResponse<TokensDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<TokensDto>>> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [Authorize]
        [HttpPost("Logout")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<bool>>> Logout()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _mediator.Send(new LogoutCommand(userId));
            return HandleResult(result);
        }

        [HttpPost("Forgot-Password")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpPost("Reset-Password")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [Authorize]
        [HttpPost("Change-Password")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var command = new ChangePasswordCommand(userId, request.OldPassword, request.NewPassword);
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// Get my profile — returns different schema based on the Role in JWT Token:
        [Authorize]
        [HttpGet("My-Profile")]
        [SwaggerResponse(StatusCodes.Status200OK, "Doctor profile", typeof(ApiResponse<DoctorDto>))]
        [SwaggerResponse(StatusCodes.Status200OK, "Student profile", typeof(ApiResponse<StudentDto>))]
        [SwaggerResponse(StatusCodes.Status200OK, "Patient profile", typeof(ApiResponse<PatientDto>))]
        [SwaggerResponse(StatusCodes.Status200OK, "Admin profile", typeof(ApiResponse<AdminDto>))]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> GetMyProfile()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return CreateErrorResponse<object>("Unauthorized: Invalid token", 401);

            var role = GetUserRoleFromToken();
            if (string.IsNullOrEmpty(role))
                return CreateErrorResponse<object>("Unauthorized: Role not found in token", 401);

            var result = await _mediator.Send(new GetMyProfileQuery(userId.Value.ToString(), role));
            return HandleResult(result);
        }

        /// Get my statistics — available for Doctor and Student only:
        [Authorize(Roles = "Doctor,Student")]
        [HttpGet("me/Statistics")]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<object>>> GetMyStatistics()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return CreateErrorResponse<object>("Unauthorized: Invalid token", 401);

            var role = GetUserRoleFromToken();
            if (string.IsNullOrEmpty(role))
                return CreateErrorResponse<object>("Unauthorized: Role not found in token", 401);

            var result = await _mediator.Send(new GetMyStatisticsQuery(userId.Value.ToString(), role));
            return HandleResult(result);
        }
    }

    public record ChangePasswordRequestDto(string OldPassword, string NewPassword);
}
