using System.Linq.Expressions;

namespace DentalHub.Infrastructure.Specification
{
    public class BaseSpecificationWithProjection<T, TResult> : BaseSpecification<T>,
        ISpecificationWithProjection<T, TResult> where T : class
    {
        public Expression<Func<T, TResult>> Projection { get; set; }

        public BaseSpecificationWithProjection(Expression<Func<T, TResult>> projection)
        {
            Projection = projection;
        }

        public BaseSpecificationWithProjection(
            Expression<Func<T, bool>> criteria,
            Expression<Func<T, TResult>> projection) : base(criteria)
        {
            Projection = projection;
        }
    }
}
