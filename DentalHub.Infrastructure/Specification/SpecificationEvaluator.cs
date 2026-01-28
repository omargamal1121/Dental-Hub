using Microsoft.EntityFrameworkCore;

namespace DentalHub.Infrastructure.Specification
{
    public class SpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(
            IQueryable<T> inputQuery,
            ISpecification<T> spec)
        {
            var query = inputQuery;

            // Apply filtering criteria
            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            // Apply includes (eager loading)
            if (spec.Includes != null && spec.Includes.Any())
                query = spec.Includes.Aggregate(query,
                    (current, include) => current.Include(include));

            // Apply string-based includes
            if (spec.IncludeStrings != null && spec.IncludeStrings.Any())
                query = spec.IncludeStrings.Aggregate(query,
                    (current, include) => current.Include(include));

            // Apply sorting
            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

            // Apply pagination
            if (spec.Paging != null)
            {
                query = query
                    .Skip(spec.Paging.Skip)
                    .Take(spec.Paging.PageSize);
            }

            return query;
        }
    }
}
