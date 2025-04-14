using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skyress.Domain.Aggregates.TagAssignmnet;

namespace Skyress.Infrastructure.Configurations
{
    public class TagAssignmentDbConfiguration : IEntityTypeConfiguration<TagAssignment>
    {
        public void Configure(EntityTypeBuilder<TagAssignment> builder)
        {
            builder.HasAlternateKey(ta => new { ta.TagId, ta.ItemId });

            builder.Property(ta => ta.TagId)
                .IsRequired();

            builder.Property(ta => ta.ItemId)
                .IsRequired();
        }
    }
} 