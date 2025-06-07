using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Newtonsoft.Json;
using Skyress.Application.Contracts.Persistence;
using Skyress.Domain.Aggregates.Customer;
using Skyress.Domain.Aggregates.Invoice;
using Skyress.Domain.Aggregates.Item;
using Skyress.Domain.Aggregates.Payment;
using Skyress.Domain.Aggregates.Tag;
using Skyress.Domain.Aggregates.TagAssignmnet;
using Skyress.Domain.Aggregates.Todo;
using Skyress.Domain.primitives;
using Skyress.Infrastructure.outbox;

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
        internal DbSet<OutboxMessage> OutboxMessages { get; set; }
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

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ConvertDomainEventsToOutboxMessage();
            return await base.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null) return;
            _transaction = await base.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                await _transaction?.CommitAsync(cancellationToken)!;
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
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

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _transaction?.RollbackAsync(cancellationToken)!;
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

        private void ConvertDomainEventsToOutboxMessage()
        {
            var messages = base.ChangeTracker.Entries<AggregateRoot>()
                .Select(e => e.Entity)
                .SelectMany(aggregateRoot =>
                {
                    var events = aggregateRoot.GetDomainEvents();
                    aggregateRoot.ClearDomainEvents();
                    return events;
                }).Select(
                    @event => new OutboxMessage()
                {
                    Id = Guid.NewGuid(),
                    OccuredOnUtc = DateTime.UtcNow,
                    Type = @event.GetType().Name,
                    Content = JsonConvert.SerializeObject(
                        @event,
                        new JsonSerializerSettings()
                        {
                            TypeNameHandling = TypeNameHandling.All,
                        })
                }).ToList();
            
            base.Set<OutboxMessage>().AddRange(messages);
        }
    }
}
