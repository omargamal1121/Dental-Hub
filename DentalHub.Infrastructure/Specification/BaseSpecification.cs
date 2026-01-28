using System.Linq.Expressions;

namespace DentalHub.Infrastructure.Specification
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>>? Criteria { get; set; }
        public Pagination? Paging { get; set; }
        public Expression<Func<T, object>>? OrderBy { get; set; }
        public Expression<Func<T, object>>? OrderByDescending { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new();
        public List<string> IncludeStrings { get; set; } = new();

        public BaseSpecification()
        {
            Includes = new List<Expression<Func<T, object>>>();
            IncludeStrings = new List<string>();
        }

        public BaseSpecification(Expression<Func<T, bool>> criteria) : this()
        {
            Criteria = criteria;
        }

        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }

        public void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        public void ApplyPaging(int page, int pageSize)
        {
            Paging = new Pagination
            {
                Page = page,
                PageSize = pageSize
            };
        }

        public void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        public void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression;
        }
    }
}
