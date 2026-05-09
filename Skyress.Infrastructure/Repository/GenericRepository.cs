using Microsoft.EntityFrameworkCore;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Primitives;
using Skyress.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Skyress.Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : AggregateRoot
    {
        protected readonly SkyressDbContext SkyressDbContext;
        protected readonly DbSet<T> DbSet;

        protected GenericRepository(SkyressDbContext skyressDbContext)
        {
            this.SkyressDbContext = skyressDbContext;
            this.DbSet = skyressDbContext.Set<T>();
        }

        public IQueryable<T> GetAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            List<Expression<Func<T, object>>>? includes = null,
            bool disableTracking = false,
            bool includeDeleted = false)
        {
            IQueryable<T> query = DbSet;

            if (disableTracking)
                query = query.AsNoTracking();

            if (includes != null)
                query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (predicate != null)
            {
                query = query.Where(predicate);
            }
            
            if (!includeDeleted && typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                Expression<Func<T, bool>> notDeleted = t => !((ISoftDeletable)t).IsDeleted;
                query = query.Where(notDeleted);
            }

            if (orderBy != null)
                query = orderBy(query);

            return query;
        }

        public async Task<T> CreateAsync(T entity, CancellationToken cancellationToken = default)
        {
            var savedEntity = await this.DbSet.AddAsync(entity, cancellationToken);
            return savedEntity.Entity;
        }

        public async Task DeleteByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            var entity = await this.GetByIdAsync(id, cancellationToken);
            if (entity == null)
            {
                return;
            }
            if (entity is ISoftDeletable softDeletable)
            {
                softDeletable.SoftDelete();
                return;
            }
            this.HardDeleteAsync(entity);
        }

        public IUnitOfWork UnitOfWork => this.SkyressDbContext;

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await GetAsync().ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
        {
            return await GetAsync(predicate: t => t.Id == id).FirstOrDefaultAsync(cancellationToken);
        }

        private void HardDeleteAsync(T entity)
        {
            this.DbSet.Remove(entity);
        }
    }
}
