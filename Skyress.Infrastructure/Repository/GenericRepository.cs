using Microsoft.EntityFrameworkCore;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.primitives;
using Skyress.Infrastructure.Persistence;


namespace Skyress.Infrastructure.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : AggregateRoot
    {
        private readonly SkyressDbContext skyressDbContext;
        private readonly DbSet<T> dbSet;

        public GenericRepository(SkyressDbContext skyressDbContext)
        {
            this.skyressDbContext = skyressDbContext;
            this.dbSet = skyressDbContext.Set<T>();
        }

        public async Task<T> CreateAsync(T entity)
        {
            var savedEntity = await this.dbSet.AddAsync(entity);
            await this.skyressDbContext.SaveChangesAsync();
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
                await this.skyressDbContext.SaveChangesAsync();
                return;
            }
            await this.HardDeleteAsync(entity);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await this.dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(long id)
        {
            return await this.dbSet.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<T> UpdateAsync(T entity)
        {
            //TODO delete and create new record
            this.skyressDbContext.Entry(entity).State = EntityState.Modified;
            await this.skyressDbContext.SaveChangesAsync();
            return entity;
        }

        private async Task HardDeleteAsync(T entity)
        {
            this.dbSet.Remove(entity);
            await this.skyressDbContext.SaveChangesAsync();
        }
    }
}
