using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skyress.Domain.Aggregates.Todo;

namespace Skyress.Infrastructure.Configurations
{
    public class TodoDbConfiguration : IEntityTypeConfiguration<Todo>
    {
        public void Configure(EntityTypeBuilder<Todo> builder)
        {
            builder.Property(t => t.State)
                .IsRequired();

            builder.Property(t => t.context)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(t => t.LastEditBy)
                .HasMaxLength(50);

            builder.Property(t => t.LastEditDate)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();
        }
    }
} 