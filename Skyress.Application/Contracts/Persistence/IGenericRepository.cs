using System.Linq.Expressions;
using Skyress.Domain.Primitives;

namespace Skyress.Application.Contracts.Persistence
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        public IUnitOfWork UnitOfWork { get; }
        public Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

        public Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

        public Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default);

        public Task DeleteByIdAsync(long id, CancellationToken cancellationToken = default);

        public IQueryable<T> GetAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            List<Expression<Func<T, object>>>? includes = null,
            bool disableTracking = false,
             bool includeDeleted = false);
    }
}
