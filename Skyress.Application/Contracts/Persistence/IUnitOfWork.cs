using Microsoft.EntityFrameworkCore.Storage;

namespace Skyress.Application.Contracts.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task<IDbContextTransaction> BeginTransactionAsync(Guid id, CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(Guid id, CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
} 