using Asp.Versioning;
using DentalHub.Application.Commands.Auth;
using DentalHub.Application.DTOs.Auth;
using DentalHub.Application.Common;
using DentalHub.Application.Queries.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using DentalHub.Application.DTOs.Shared;

namespace DentalHub.API.Controllers.v1
{
    [ApiVersion("1.0")]
    public class AuthController : BaseController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator) : base()
        {
            _mediator = mediator;
        }

        /// <summary>Authenticate a user and receive a JWT access token plus a refresh token.</summary>
        [HttpPost("Login")]
        [ProducesResponseType(typeof(ApiResponse<TokensDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<TokensDto>>> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>
        /// Refresh an expired access token using a valid refresh token.
        /// Returns a new access token and a new refresh token (rotation).
        /// The submitted refresh token is invalidated immediately after use.
        /// </summary>
        [HttpPost("Refresh")]
        [ProducesResponseType(typeof(ApiResponse<TokensDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<TokensDto>>> Refresh([FromBody] RefreshTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest(ApiResponse<object>.CreateErrorResponse(
                    "RefreshToken is required",
                    new ErrorResponse("Validation", "RefreshToken is required"),
                    400));

            var result = await _mediator.Send(new RefreshTokenCommand(request.RefreshToken));
            return HandleResult(result);
        }

        /// <summary>
        /// Log out from the current device.
        /// Revokes the provided refresh token and rotates the user's SecurityStamp.
        /// </summary>
        [Authorize]
        [HttpPost("Logout")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<bool>>> Logout([FromBody] LogoutRequest request)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var result = await _mediator.Send(new LogoutCommand(userId, request.RefreshToken ?? string.Empty));
            return HandleResult(result);
        }

        /// <summary>
        /// Log out from ALL devices.
        /// Rotates the SecurityStamp (invalidating all JWTs) and revokes every active refresh token.
        /// </summary>
        [Authorize]
        [HttpPost("Logout-All")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<bool>>> LogoutAll()
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var result = await _mediator.Send(new LogoutFromAllDevicesCommand(userId));
            return HandleResult(result);
        }

        /// <summary>Initiate a password reset via email.</summary>
        [HttpPost("Forgot-Password")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>Complete a password reset using the token received by email.</summary>
        [HttpPost("Reset-Password")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        /// <summary>Change password while authenticated.</summary>
        [Authorize]
        [HttpPost("Change-Password")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty) return Unauthorized();

            var command = new ChangePasswordCommand(userId, request.OldPassword, request.NewPassword);
            var result  = await _mediator.Send(command);
            return HandleResult(result);
        }
    }

    public record ChangePasswordRequestDto(string OldPassword, string NewPassword);
}
