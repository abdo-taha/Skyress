using System.Threading.Tasks;

namespace Skyress.Application.Contracts.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IItemRepository ItemRepository { get; }
        ICustomerRepository CustomerRepository { get; }
        IInvoiceRepository InvoiceRepository { get; }
        IPaymentRepository PaymentRepository { get; }
        ITagRepository TagRepository { get; }
        ITagAssignmentRepository TagAssignmentRepository { get; }
        ITodoRepository TodoRepository { get; }

        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
} 