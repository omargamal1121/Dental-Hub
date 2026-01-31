using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Common
{


public class Result<T> : Result
{
    public T Value { get; }
		public bool IsSuccess { get; }
		public bool IsFailure => !IsSuccess;
		public string Error { get; }


		private Result(bool isSuccess, T value, string error)
      
    {
            IsSuccess = isSuccess;
            Error = error;


			Value = value;
    }

    public static Result<T> Success(T value)
        => new(true, value, null);

    public static Result<T> Failure(string error)
        => new(false, default!, error);
}

}

