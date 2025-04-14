using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skyress.Domain.Aggregates.Payment;

namespace Skyress.Infrastructure.Configurations
{
    public class InstallmentDbConfiguration : IEntityTypeConfiguration<Installment>
    {
        public void Configure(EntityTypeBuilder<Installment> builder)
        {
            builder.Property(i => i.PaymentId)
                .IsRequired();

            builder.Property(i => i.DueAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.number)
                .IsRequired();

            builder.Property(i => i.DueDate)
                .IsRequired();

            builder.Property(i => i.paymentState)
                .IsRequired();

            builder.Property(i => i.IsDeleted)
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