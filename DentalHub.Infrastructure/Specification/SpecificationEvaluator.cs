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

            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            if (spec.Includes != null && spec.Includes.Any())
                query = spec.Includes.Aggregate(query,
                    (current, include) => current.Include(include));

         
            if (spec.IncludeStrings != null && spec.IncludeStrings.Any())
                query = spec.IncludeStrings.Aggregate(query,
                    (current, include) => current.Include(include));

          
            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);

     
            if (spec.Paging != null)
            {
                query = query
                    .Skip(spec.Paging.Skip)
                    .Take(spec.Paging.PageSize);
            }

            return query;
        }
		public static IQueryable<T> GetCountQuery(
	IQueryable<T> inputQuery,
	ISpecification<T> spec)
		{
			var query = inputQuery;

			if (spec.Criteria != null)
				query = query.Where(spec.Criteria);

			return query;
		}

	}
}
