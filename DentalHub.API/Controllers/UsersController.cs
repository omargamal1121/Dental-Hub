using DentalHub.Application.Commands.Auth;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Admins;
using DentalHub.Application.DTOs.Doctors;
using DentalHub.Application.DTOs.Patients;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.DTOs.Students;
using DentalHub.Application.Queries.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace DentalHub.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator) : base()
        {
            _mediator = mediator;
        }

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

            var result = await _mediator.Send(new GetMyProfileQuery(userId.Value, role));
            return HandleResult(result);
        }

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

            Guid.TryParse(userId.ToString(), out var userGuid);
            var result = await _mediator.Send(new GetMyStatisticsQuery(userGuid, role));
            return HandleResult(result);
        }

        [HttpDelete()]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<object>>> DeleteUser()
        {
            var userId = GetUserIdFromToken();
            if (userId == null)
                return CreateErrorResponse<object>("Unauthorized: Invalid token", 401);
			var result = await _mediator.Send(new DeleteUserCommand(userId.Value));
            return HandleResult(result);
        }
    }
}
