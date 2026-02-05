using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DentalHub.API.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILinkBuilder? _linkBuilder;
       
        protected BaseController(ILinkBuilder? linkBuilder = null)
        {
            _linkBuilder = linkBuilder;
        }

        protected List<string> GetModelErrors()
        {
            return ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();
        }

        protected ActionResult<ApiResponse<T>> CreateErrorResponse<T>(
             string message,
             int statusCode,
             List<string>? errors = null)
        {
            var errorResponse = ApiResponse<T>.CreateErrorResponse(
                message,
                new ErrorResponse("Error", errors ?? new List<string> { message }),
                statusCode
            );
            return StatusCode(statusCode, errorResponse);
        }

        protected string GetUserId() =>
            HttpContext?.Items?["UserId"]?.ToString() ?? string.Empty;

        protected bool HasManagementRole() =>
            User?.IsInRole("Admin") == true || User?.IsInRole("SuperAdmin") == true || User?.IsInRole("DeliveryCompany") == true;

        protected ActionResult<ApiResponse<T>> HandleResult<T>(
          Result<T> result,
          string apiName = "",
          int? id = null)
        {
            var links = _linkBuilder?.MakeRelSelf(_linkBuilder.GenerateLinks(id), apiName);

            ApiResponse<T> apiResponse;

            if (result.IsSuccess)
            {
                apiResponse = ApiResponse<T>.CreateSuccessResponse(
                    result.Message ?? "Success",
                    result.Data,
                    result.Status,
                    warnings: null, // Result<T> in DentalHub doesn't have warnings
                    links: links);
            }
            else
            {
                var errorResponse = (result.Errors != null && result.Errors.Count > 0)
                    ? new ErrorResponse("Error", result.Errors)
                    : new ErrorResponse("Error", result.Message ?? "An error occurred");

                apiResponse = ApiResponse<T>.CreateErrorResponse(
                    result.Message ?? "Error",
                    errorResponse,
                    result.Status,
                    warnings: null,
                    links: links);
            }

            return result.Status switch
            {
                200 => Ok(apiResponse),
                201 => StatusCode(201, apiResponse),
                400 => BadRequest(apiResponse),
                401 => Unauthorized(apiResponse),
                403 => Forbid(),
                404 => NotFound(apiResponse),
                409 => Conflict(apiResponse),
                500 => StatusCode(500, apiResponse),
                _ => StatusCode(result.Status, apiResponse)
            };
        }
        
        // Overload for non-generic Result
         protected ActionResult<ApiResponse<object>> HandleResult(
          Result result,
          string apiName = "",
          int? id = null)
        {
             var links = _linkBuilder?.MakeRelSelf(_linkBuilder.GenerateLinks(id), apiName);

            ApiResponse<object> apiResponse;

            if (result.IsSuccess)
            {
                apiResponse = ApiResponse<object>.CreateSuccessResponse(
                    result.Message ?? "Success",
                    null,
                    result.Status,
                    warnings: null,
                    links: links);
            }
            else
            {
                var errorResponse = (result.Errors != null && result.Errors.Count > 0)
                    ? new ErrorResponse("Error", result.Errors)
                    : new ErrorResponse("Error", result.Message ?? "An error occurred");

                apiResponse = ApiResponse<object>.CreateErrorResponse(
                    result.Message ?? "Error",
                    errorResponse,
                    result.Status,
                    warnings: null,
                    links: links);
            }

            return result.Status switch
            {
                200 => Ok(apiResponse),
                201 => StatusCode(201, apiResponse),
                400 => BadRequest(apiResponse),
                401 => Unauthorized(apiResponse),
                403 => Forbid(),
                404 => NotFound(apiResponse),
                409 => Conflict(apiResponse),
                 500 => StatusCode(500, apiResponse),
                _ => StatusCode(result.Status, apiResponse)
            };
        }
    }
}
