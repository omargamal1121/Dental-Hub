using System.Linq.Expressions;

namespace DentalHub.Infrastructure.Specification
{
    public interface ISpecificationWithProjection<T, TResult> : ISpecification<T>
    {
        Expression<Func<T, TResult>> Projection { get; set; }
    }
}
