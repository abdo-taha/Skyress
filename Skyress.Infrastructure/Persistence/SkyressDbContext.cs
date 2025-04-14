using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
    public class SkyressDbContext(DbContextOptions<SkyressDbContext> options) : DbContext(options)
    {
        internal DbSet<Item> Items { get; set; }
        internal DbSet<Customer> Customers { get; set; }
        internal DbSet<Invoice> Invoices { get; set; }
        internal DbSet<Payment> Payments { get; set; }
        internal DbSet<Tag> Tags { get; set; }
        internal DbSet<TagAssignment> TagAssignments { get; set; }
        internal DbSet<Todo> Todos { get; set; }
        internal DbSet<SoldItem> soldItems { get; set; }
        internal DbSet<PricingHistory> pricingHistories { get; set; }
        internal DbSet<Installment> Installments { get; set; }

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
    }
}
