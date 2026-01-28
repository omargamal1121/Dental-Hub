using DentalHub.Infrastructure.ContextAndConfig;
using DentalHub.Infrastructure.Specification;
using Microsoft.EntityFrameworkCore;

namespace DentalHub.Infrastructure.Repository
{
    public class MainRepository<T> : IMainRepository<T> where T : class
    {
        private readonly ContextApp _context;
        private readonly DbSet<T> _dbSet;

        public MainRepository(ContextApp context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        // ========== Read Operations with Projection ==========

        public async Task<List<TResult>> GetAllAsync<TResult>(
            ISpecificationWithProjection<T, TResult> specification)
        {
            var query = _dbSet.AsQueryable();
            return await SpecificationWithProjectionEvaluator<T, TResult>
                .GetQuery(query, specification)
                .ToListAsync();
        }

        public async Task<TResult?> GetByIdAsync<TResult>(
            ISpecificationWithProjection<T, TResult> specification)
        {
            var query = _dbSet.AsQueryable();
            return await SpecificationWithProjectionEvaluator<T, TResult>
                .GetQuery(query, specification)
                .FirstOrDefaultAsync();
        }

        // ========== Read Operations without Projection ==========

        public async Task<List<T>> GetAllAsync(ISpecification<T> specification)
        {
            var query = _dbSet.AsQueryable();
            return await SpecificationEvaluator<T>
                .GetQuery(query, specification)
                .ToListAsync();
        }

        public async Task<T?> GetByIdAsync(ISpecification<T> specification)
        {
            var query = _dbSet.AsQueryable();
            return await SpecificationEvaluator<T>
                .GetQuery(query, specification)
                .FirstOrDefaultAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<int> CountAsync(ISpecification<T> specification)
        {
            var query = _dbSet.AsQueryable();

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            return await query.CountAsync();
        }

        public async Task<bool> AnyAsync(ISpecification<T> specification)
        {
            var query = _dbSet.AsQueryable();

            if (specification.Criteria != null)
                query = query.Where(specification.Criteria);

            return await query.AnyAsync();
        }

        // ========== Write Operations ==========

        public async Task<T> AddAsync(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            await _dbSet.AddAsync(item);
            return item;
        }

        public async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> items)
        {
            if (items == null || !items.Any())
                throw new ArgumentNullException(nameof(items));

            await _dbSet.AddRangeAsync(items);
            return items;
        }

        public void Update(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _dbSet.Update(item);
        }

        public void Remove(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _dbSet.Remove(item);
        }

        public void RemoveRange(IEnumerable<T> items)
        {
            if (items == null || !items.Any())
                throw new ArgumentNullException(nameof(items));

            _dbSet.RemoveRange(items);
        }

        public async Task<int> ExecuteDeleteAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ExecuteDeleteAsync();
        }
    }
}
