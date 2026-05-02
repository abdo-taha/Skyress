using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Skyress.Domain.Aggregates.Auth;

namespace Skyress.Infrastructure.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		builder.ToTable("users");

		builder.HasKey(u => u.Id);

		builder.Property(u => u.Email)
			.IsRequired()
			.HasMaxLength(255);

		builder.HasIndex(u => u.Email)
			.IsUnique();

		builder.Property(u => u.PasswordHash)
			.IsRequired()
			.HasMaxLength(255);

		builder.Property(u => u.IsActive)
			.IsRequired();

		builder.HasMany(u => u.UserRoles)
			.WithOne(ur => ur.User)
			.HasForeignKey(ur => ur.UserId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasMany(u => u.RefreshTokens)
			.WithOne(rt => rt.User)
			.HasForeignKey(rt => rt.UserId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}
