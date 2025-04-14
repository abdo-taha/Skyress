using Skyress.Domain.primitives;

namespace Skyress.Application.Contracts.Persistence
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        public Task<IReadOnlyList<T>> GetAllAsync();

        public Task<T?> GetByIdAsync(long id);

        public Task<T> CreateAsync(T entity);

        public Task<T> UpdateAsync(T entity);

        public Task DeleteByIdAsync(long id);
    }
}
