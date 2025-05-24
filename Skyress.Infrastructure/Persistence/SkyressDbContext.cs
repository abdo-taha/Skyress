using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Customer;
using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Aggregates.Payment;
using Skyress.Domain.Aggregates.Tag;
using Skyress.Domain.Aggregates.TagAssignmnet;
using Skyress.Domain.Aggregates.Todo;
using Skyress.Domain.primitives;

namespace Skyress.Infrastructure.Persistence
{
    public class SkyressDbContext(DbContextOptions<SkyressDbContext> options) : DbContext(options), IUnitOfWork
    {
        internal DbSet<Item> Items { get; set; }
        internal DbSet<Customer> Customers { get; set; }
        internal DbSet<Invoice> Invoices { get; set; }
        internal DbSet<Payment> Payments { get; set; }
        internal DbSet<Tag> Tags { get; set; }
        internal DbSet<TagAssignment> TagAssignments { get; set; }
        internal DbSet<Todo> Todos { get; set; }
        internal DbSet<SoldItem> SoldItems { get; set; }
        internal DbSet<PricingHistory> PricingHistories { get; set; }
        internal DbSet<Installment> Installments { get; set; }
        private IDbContextTransaction? _transaction;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(BaseEntity.Id))
                    .UseIdentityColumn()
                    .HasIdentityOptions(startValue: 100000, incrementBy: 1);
            }

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(SkyressDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null) return;
            _transaction = await base.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await SaveChangesAsync();
                await _transaction?.CommitAsync()!;
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
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

        public override void Dispose()
        {
            base.Dispose();
            _transaction?.Dispose();
        }
    }
}
