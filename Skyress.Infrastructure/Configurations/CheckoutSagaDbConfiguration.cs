using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skyress.Application.Checkout.Sagas;

namespace Skyress.Infrastructure.Configurations;

public class CheckoutSagaDbConfiguration : IEntityTypeConfiguration<CheckoutSagaData>
{
    public void Configure(EntityTypeBuilder<CheckoutSagaData> builder)
    {
        builder.HasKey(d => d.CorrelationId);
    }
}