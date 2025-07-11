using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skyress.Domain.Aggregates.Basket;

namespace Skyress.Infrastructure.Configurations;

public class BasketItemDbConfiguration : IEntityTypeConfiguration<BasketItem>
{
    public void Configure(EntityTypeBuilder<BasketItem> builder)
    {
        builder.HasKey(bi => bi.Id);

        builder.HasOne<Basket>()
            .WithMany(b => b.BasketItems)
            .HasForeignKey(bi => bi.BasketId);

    }
}