using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skyress.Domain.Aggregates.Item;

namespace Skyress.Infrastructure.Configurations
{
    public class ItemDbConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(i => i.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(i => i.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.CostPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.QuantityLeft)
                .IsRequired();

            builder.Property(i => i.QuantitySold)
                .IsRequired();

            builder.Property(i => i.QrCode)
                .HasMaxLength(100);

            builder.Property(i => i.Unit)
                .IsRequired();

            builder.Property(i => i.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(i => i.LastEditBy)
                .HasMaxLength(50);

            builder.Property(i => i.LastEditDate)
                .IsRequired();

            builder.Property(i => i.CreaedAt)
                .IsRequired()
                .ValueGeneratedOnAdd();
        }
    }
}
