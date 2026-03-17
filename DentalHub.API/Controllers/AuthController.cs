using DentalHub.Application.Commands.Auth;
using DentalHub.Application.DTOs.Auth;
using DentalHub.Application.Common;
using DentalHub.Application.Queries.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using DentalHub.Application.DTOs.Shared;

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
            if (userId == Guid.Empty) return Unauthorized();

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
            if (userId == Guid.Empty) return Unauthorized();

            var command = new ChangePasswordCommand(userId, request.OldPassword, request.NewPassword);
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

    }

    public record ChangePasswordRequestDto(string OldPassword, string NewPassword);
}
