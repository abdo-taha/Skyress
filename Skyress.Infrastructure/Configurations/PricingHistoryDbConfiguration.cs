using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skyress.Domain.Aggregates.Item;

namespace Skyress.Infrastructure.Configurations
{
    public class PricingHistoryDbConfiguration : IEntityTypeConfiguration<PricingHistory>
    {
        public void Configure(EntityTypeBuilder<PricingHistory> builder)
        {
            builder.Property(p => p.ItemId)
                .IsRequired();

            builder.Property(p => p.OldPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.NewPrice)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.OldCost)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.NewCost)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.pricingChangeType)
                .IsRequired();

            builder.Property(p => p.LastEditBy)
                .HasMaxLength(50);

            builder.Property(p => p.LastEditDate)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .ValueGeneratedOnAdd();
        }
    }
} 