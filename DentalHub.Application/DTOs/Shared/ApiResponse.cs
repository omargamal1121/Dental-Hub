namespace DentalHub.Application.DTOs.Shared
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public int StatusCode { get; set; }
        public List<string> Warnings { get; set; }
        public ErrorResponse Error { get; set; }
        public object Links { get; set; }

        public static ApiResponse<T> CreateSuccessResponse(string message, T data, int statusCode, List<string> warnings = null, object links = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data,
                StatusCode = statusCode,
                Warnings = warnings,
                Links = links
            };
        }

        public static ApiResponse<T> CreateErrorResponse(string message, ErrorResponse error, int statusCode, List<string> warnings = null, object links = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Error = error,
                StatusCode = statusCode,
                Warnings = warnings,
                Links = links
            };
        }
    }

    public class ErrorResponse
    {
        public string Type { get; set; }
        public List<string> Errors { get; set; }

        public ErrorResponse(string type, List<string> errors)
        {
            Type = type;
            Errors = errors;
        }

        public ErrorResponse(string type, string error)
        {
            Type = type;
            Errors = new List<string> { error };
        }
    }
}
