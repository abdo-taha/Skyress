using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skyress.Domain.Aggregates.Customer;

namespace Skyress.Infrastructure.Configurations
{
    public class CustomerDbConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.State)
                .IsRequired();

            builder.Property(c => c.Notes)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.LastEditBy)
                .HasMaxLength(50);

            builder.Property(c => c.LastEditDate)
                .IsRequired();

            builder.Property(c => c.CreaedAt)
                .IsRequired()
                .ValueGeneratedOnAdd();
        }
    }
} 