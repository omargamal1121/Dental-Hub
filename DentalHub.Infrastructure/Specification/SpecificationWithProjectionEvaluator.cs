using Microsoft.EntityFrameworkCore;

namespace DentalHub.Infrastructure.Specification
{
    public class SpecificationWithProjectionEvaluator<T, TResult> where T : class
    {
        public static IQueryable<TResult> GetQuery(
            IQueryable<T> inputQuery,
            ISpecificationWithProjection<T, TResult> spec)
        {
            var query = inputQuery;

            // Apply filtering criteria
            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            // Apply includes (eager loading) - before projection
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

            // Apply projection (Select)
            var projectedQuery = query.Select(spec.Projection);

            // Apply pagination after projection
            if (spec.Paging != null)
                projectedQuery = projectedQuery
                    .Skip(spec.Paging.Skip)
                    .Take(spec.Paging.PageSize);

            return projectedQuery;
        }
    }
}
