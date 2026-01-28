using DentalHub.Infrastructure.Specification;

namespace DentalHub.Infrastructure.Repository
{
    public interface IMainRepository<T> where T : class
    {
        // Read operations with projection (returns DTOs)
        Task<List<TResult>> GetAllAsync<TResult>(
            ISpecificationWithProjection<T, TResult> specification);
        Task<TResult?> GetByIdAsync<TResult>(
            ISpecificationWithProjection<T, TResult> specification);

        // Read operations without projection (returns entities)
        Task<List<T>> GetAllAsync(ISpecification<T> specification);
        Task<T?> GetByIdAsync(ISpecification<T> specification);
        Task<T?> GetByIdAsync(Guid id);
        Task<int> CountAsync(ISpecification<T> specification);
        Task<bool> AnyAsync(ISpecification<T> specification);

        // Write operations
        Task<T> AddAsync(T item);
        Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> items);
        void Update(T item);
        void Remove(T item);
        void RemoveRange(IEnumerable<T> items);
        Task<int> ExecuteDeleteAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate);
    }
}
