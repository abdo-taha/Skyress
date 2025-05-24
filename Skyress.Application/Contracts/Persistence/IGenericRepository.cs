using System.Linq.Expressions;
using Skyress.Domain.primitives;

namespace Skyress.Application.Contracts.Persistence
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        public IUnitOfWork UnitOfWork { get; }
        public Task<IReadOnlyList<T>> GetAllAsync();

        public Task<T?> GetByIdAsync(long id);

        public Task<T> CreateAsync(T entity);

        public Task DeleteByIdAsync(long id);

        public IQueryable<T> GetAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            List<Expression<Func<T, object>>>? includes = null,
            bool disableTracking = false,
             bool includeDeleted = false);
    }
}
