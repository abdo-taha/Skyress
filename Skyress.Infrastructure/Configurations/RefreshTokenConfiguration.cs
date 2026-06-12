using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skyress.Domain.Aggregates.Auth;

namespace Skyress.Infrastructure.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
	public void Configure(EntityTypeBuilder<RefreshToken> builder)
	{
		builder.ToTable("refresh_tokens");

		builder.HasKey(rt => rt.Id);

		builder.Property(rt => rt.Token)
			.IsRequired()
			.HasMaxLength(500);

		builder.Property(rt => rt.TokenHash)
			.IsRequired()
			.HasMaxLength(128);

		builder.HasIndex(rt => rt.TokenHash)
			.IsUnique();

		builder.Property(rt => rt.RowVersion)
			.IsRowVersion();

		builder.HasIndex(rt => rt.FamilyId);

		builder.HasIndex(rt => rt.UserId);

		builder.HasOne(rt => rt.User)
			.WithMany(u => u.RefreshTokens)
			.HasForeignKey(rt => rt.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
