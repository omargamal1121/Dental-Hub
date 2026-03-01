using FluentValidation;
using MediatR;
using DentalHub.Application.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DentalHub.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
                var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

                if (failures.Count != 0)
                {
                    var errors = failures.Select(f => f.ErrorMessage).Distinct().ToList();
                    
                    // We assume TResponse is Result<T> or Result
                    if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                    {
                        var resultType = typeof(TResponse).GetGenericArguments()[0];
                        var failureMethod = typeof(Result<>).MakeGenericType(resultType).GetMethod("Failure", new[] { typeof(List<string>), typeof(int) });
                        if (failureMethod != null)
                        {
                            return (TResponse)failureMethod.Invoke(null, new object[] { errors, 400 })!;
                        }
                    }
                    else if (typeof(TResponse) == typeof(Result))
                    {
                        var failureMethod = typeof(Result).GetMethod("Failure", new[] { typeof(List<string>), typeof(int) });
                        if (failureMethod != null)
                        {
                            return (TResponse)failureMethod.Invoke(null, new object[] { errors, 400 })!;
                        }
                    }
                    else
                    {
                        // Fallback for custom response types like CreateAdminCommandResponse
                        var response = System.Activator.CreateInstance<TResponse>();
                        var successProp = typeof(TResponse).GetProperty("Success");
                        var errorsProp = typeof(TResponse).GetProperty("Errors");
                        var messageProp = typeof(TResponse).GetProperty("Message");

                        if (successProp != null) successProp.SetValue(response, false);
                        if (errorsProp != null && errorsProp.PropertyType == typeof(List<string>)) errorsProp.SetValue(response, errors);
                        if (messageProp != null) messageProp.SetValue(response, "Validation failed");

                        if (successProp != null || errorsProp != null)
                        {
                            return response;
                        }
                    }
                    
                    throw new ValidationException(failures);
                }
            }
            return await next();
        }
    }
}
