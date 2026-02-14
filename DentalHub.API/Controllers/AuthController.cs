using DentalHub.Application.Commands.Auth;
using DentalHub.Application.DTOs.Auth;
using DentalHub.Application.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
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

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<TokensDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<TokensDto>>> Login([FromBody] LoginCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        //[HttpPost("refresh-token")]
        //public async Task<ActionResult<ApiResponse<TokensDto>>> RefreshToken()
        //{
        //    var command = new RefreshTokenCommand();
        //    var result = await _mediator.Send(command);
        //    return HandleResult(result);
        //}

        [Authorize]
        [HttpPost("logout")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<bool>>> Logout()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var result = await _mediator.Send(new LogoutCommand(userId));
            return HandleResult(result);
        }

        [HttpPost("forgot-password")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [HttpPost("reset-password")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<bool>>> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return HandleResult(result);
        }

        [Authorize]
        [HttpPost("change-password")]
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
    }

    public record ChangePasswordRequestDto(string OldPassword, string NewPassword);
}
