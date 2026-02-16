using System.Linq.Expressions;

namespace DentalHub.Application.Specification.Comman
{
    public interface ISpecificationWithProjection<T, TResult> : ISpecification<T>
    {
        Expression<Func<T, TResult>> Projection { get; set; }
    }
}
