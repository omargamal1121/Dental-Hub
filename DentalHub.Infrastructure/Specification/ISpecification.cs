using System.Linq.Expressions;

namespace DentalHub.Infrastructure.Specification
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>>? Criteria { get; set; }
        Pagination? Paging { get; set; }
        Expression<Func<T, object>>? OrderBy { get; set; }
        Expression<Func<T, object>>? OrderByDescending { get; set; }
        List<Expression<Func<T, object>>>? Includes { get; set; }
        List<string> IncludeStrings { get; set; }
    }
}
