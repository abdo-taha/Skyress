using Microsoft.EntityFrameworkCore.Storage;
using Skyress.Application.Contracts.Persistence;

namespace Skyress.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SkyressDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(
            SkyressDbContext context,
            IItemRepository itemRepository,
            ICustomerRepository customerRepository,
            IInvoiceRepository invoiceRepository,
            IPaymentRepository paymentRepository,
            ITagRepository tagRepository,
            ITagAssignmentRepository tagAssignmentRepository,
            ITodoRepository todoRepository)
        {
            _context = context;
            ItemRepository = itemRepository;
            CustomerRepository = customerRepository;
            InvoiceRepository = invoiceRepository;
            PaymentRepository = paymentRepository;
            TagRepository = tagRepository;
            TagAssignmentRepository = tagAssignmentRepository;
            TodoRepository = todoRepository;
            _transaction = null;
        }

        public IItemRepository ItemRepository { get; }
        public ICustomerRepository CustomerRepository { get; }
        public IInvoiceRepository InvoiceRepository { get; }
        public IPaymentRepository PaymentRepository { get; }
        public ITagRepository TagRepository { get; }
        public ITagAssignmentRepository TagAssignmentRepository { get; }
        public ITodoRepository TodoRepository { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _transaction?.CommitAsync()!;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _transaction?.RollbackAsync()!;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            _context.Dispose();
            _transaction?.Dispose();
        }
    }
} 