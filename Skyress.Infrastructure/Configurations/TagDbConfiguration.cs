using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skyress.Domain.Aggregates.Tag;

namespace Skyress.Infrastructure.Configurations
{
    public class TagDbConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Type)
                .IsRequired();

            builder.Property(t => t.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);
        }
    }
} 