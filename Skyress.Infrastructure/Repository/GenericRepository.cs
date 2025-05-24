using Microsoft.EntityFrameworkCore;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.primitives;
using Skyress.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace Skyress.Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : AggregateRoot
    {
        protected readonly SkyressDbContext skyressDbContext;
        protected readonly DbSet<T> dbSet;

        protected GenericRepository(SkyressDbContext skyressDbContext)
        {
            this.skyressDbContext = skyressDbContext;
            this.dbSet = skyressDbContext.Set<T>();
        }

        public IQueryable<T> GetAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            List<Expression<Func<T, object>>>? includes = null,
            bool disableTracking = false)
        {
            IQueryable<T> query = dbSet;

            if (disableTracking)
                query = query.AsNoTracking();

            if (includes != null)
                query = includes.Aggregate(query, (current, include) => current.Include(include));

            if (predicate != null)
                query = query.Where(predicate);

            if (orderBy != null)
                query = orderBy(query);

            return query;
        }

        public async Task<T> CreateAsync(T entity)
        {
            var savedEntity = await this.dbSet.AddAsync(entity);
            return savedEntity.Entity;
        }

        public async Task DeleteByIdAsync(long id)
        {
            var entity = await this.GetByIdAsync(id);
            if (entity == null)
            {
                return;
            }
            if (entity is ISoftDeletable softDeletable)
            {
                softDeletable.IsDeleted = true;
                return;
            }
            this.HardDeleteAsync(entity);
        }

        public IUnitOfWork UnitOfWork => this.skyressDbContext;

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await GetAsync().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(long id)
        {
            return await GetAsync(predicate: t => t.Id == id).FirstOrDefaultAsync();
        }

        public T UpdateAsync(T entity)
        {
            this.skyressDbContext.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        private void HardDeleteAsync(T entity)
        {
            this.dbSet.Remove(entity);
        }
    }
}
