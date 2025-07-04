using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skyress.Domain.Aggregates.Payment;

namespace Skyress.Infrastructure.Configurations
{
    public class PaymentDbConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.Property(p => p.TotalPaid)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.TotalDue)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.InvoiceId)
                .IsRequired();

            builder.Property(p => p.PaymentType)
                .IsRequired();

            builder.Property(p => p.PaymentState)
                .IsRequired();

            builder.Property(p => p.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(p => p.LastEditBy)
                .HasMaxLength(50);

            builder.Property(p => p.LastEditDate)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired();
        }
    }
} 