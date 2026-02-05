namespace DentalHub.Application.Common
{
 
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
		public int Status { get; set; }

		// Success Result
		public static Result<T> Success(T data, string? message = null,int status=200)
        {
            return new Result<T>
            {
                Errors = null,
                Status = status,
				IsSuccess = true,
                Data = data,
                Message = message
            };
        }

        // Failure Result
        public static Result<T> Failure(string error,int status=400)
        {
            return new Result<T>
            {
                Status = status,
				IsSuccess = false,
                Errors = new List<string> { error }
            };
        }

        public static Result<T> Failure(List<string> errors, int status = 400)
        {
            return new Result<T>
            {
                Status = status,
				IsSuccess = false,
                Errors = errors
            };
        }
    }


    public class Result
    {
		public int Status { get; set; }
		public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }

        public static Result Success(string? message = null,int status= 200)
        {
            return new Result
            {
                IsSuccess = true,
                Message = message
            };
        }

        public static Result Failure(string error, int status = 400)
        {
            return new Result
            {
                Status = status,
				IsSuccess = false,
                Errors = new List<string> { error }
            };
        }

        public static Result Failure(List<string> errors, int status = 400)
        {
            return new Result
            {
                Status = status,
				IsSuccess = false,
                Errors = errors
            };
        }
    }
}
