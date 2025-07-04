using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skyress.Domain.Aggregates.Invoice;

namespace Skyress.Infrastructure.Configurations
{
    public class SoldItemDbConfiguration : IEntityTypeConfiguration<SoldItem>
    {
        public void Configure(EntityTypeBuilder<SoldItem> builder)
        {
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(s => s.Quantity)
                .IsRequired();

            builder.Property(s => s.TransactionType)
                .IsRequired();

            builder.Property(s => s.SellingTime)
                .IsRequired();

            builder.Property(s => s.InvoiceId)
                .IsRequired();

            builder.Property(s => s.ItemId);

            builder.Property(s => s.ItemCost)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(s => s.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(s => s.LastEditBy)
                .HasMaxLength(50);

            builder.Property(s => s.LastEditDate)
                .IsRequired();

            builder.Property(s => s.CreatedAt)
                .IsRequired();
        }
    }
} 