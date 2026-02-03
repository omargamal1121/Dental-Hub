using DentalHub.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace DentalHub.API.Middleware
{
    /// <summary>
    /// Global exception handler middleware to catch and handle all unhandled exceptions
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            // ✅ حددي الـ response type
            ErrorResponse response;

            switch (exception)
            {
                case NotFoundException notFoundEx:
                    response = new ErrorResponse
                    {
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Message = notFoundEx.Message,
                        IsSuccess = false
                    };
                    break;

                case ValidationException validationEx:
                    response = new ErrorResponse
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = "Validation failed",
                        Errors = validationEx.Errors,
                        IsSuccess = false
                    };
                    break;

                case BusinessRuleException businessEx:
                    response = new ErrorResponse
                    {
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Message = businessEx.Message,
                        IsSuccess = false
                    };
                    break;

                case ForbiddenAccessException: // ✅ غيري الاسم
                    response = new ErrorResponse
                    {
                        StatusCode = (int)HttpStatusCode.Forbidden,
                        Message = "You are not authorized to perform this action",
                        IsSuccess = false
                    };
                    break;

                default:
                    response = new ErrorResponse
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError,
                        Message = "An internal server error occurred",
                        IsSuccess = false
                    };
                    break;
            }

            context.Response.StatusCode = response.StatusCode;

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    // ✅ Error Response Class
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
        public object? Errors { get; set; } // للـ ValidationException
    }

    /// <summary>
    /// Extension method to register the global exception handler middleware
    /// </summary>
    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(
            this IApplicationBuilder app)
        {
            return app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
