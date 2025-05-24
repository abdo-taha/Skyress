using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skyress.Domain.Aggregates.Invoice;

namespace Skyress.Infrastructure.Configurations
{
    public class InvoiceDbConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.Property(i => i.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.CustomerId);

            builder.Property(i => i.LastEditBy)
                .HasMaxLength(50);

            builder.Property(i => i.LastEditDate)
                .IsRequired();

            builder.Property(i => i.CreatedAt)
                .IsRequired()
                .ValueGeneratedOnAdd();

            builder.Property(i => i.IsDeleted)
                .HasDefaultValue(false);
        }
    }
} 