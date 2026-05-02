using Skyress.Domain.primitives;

namespace Skyress.Domain.Aggregates.Auth;

public class User : AggregateRoot, IAuditable
{
	public string Email { get; set; } = string.Empty;
	public string PasswordHash { get; set; } = string.Empty;
	public bool IsActive { get; set; }
	public ICollection<UserRole> UserRoles { get; set; } = [];
	public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
	public DateTime CreatedAt { get; set; }
	public string? LastEditBy { get; set; }
	public DateTime LastEditDate { get; set; }

	public User()
	{
	}

	public User(string email, string passwordHash)
	{
		Email = email;
		PasswordHash = passwordHash;
		IsActive = true;
		CreatedAt = DateTime.UtcNow;
		LastEditDate = DateTime.UtcNow;
	}
}
