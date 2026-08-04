
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Shared;
using System.Security.Claims;


namespace DentalHub.API.Middleware
{
	public class UserAuthenticationMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<UserAuthenticationMiddleware> _logger;

		public UserAuthenticationMiddleware(RequestDelegate next, ILogger<UserAuthenticationMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			_logger.LogInformation("Executing UserAuthenticationMiddleware");


			if (!context.User.Identity?.IsAuthenticated ?? true)
			{
				await _next(context);
				return;
			}

			var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userId))
			{
				_logger.LogWarning("Authenticated user without UserId claim");
				context.Response.StatusCode = StatusCodes.Status401Unauthorized;
				await context.Response.WriteAsJsonAsync(
	   ApiResponse<object>.CreateErrorResponse(
		   "Authentication Failed",
		  new Application.DTOs.Shared.ErrorResponse("Authenticated", "User not properly authenticated"),404
	   )
   );

				return;
			}

			context.Items["UserId"] = userId;
			await _next(context);
		}
	}
	public static class UserAuthenticationMiddlewareExtensions
	{
		public static IApplicationBuilder UseUserAuthentication(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<UserAuthenticationMiddleware>();
		}
	}
}