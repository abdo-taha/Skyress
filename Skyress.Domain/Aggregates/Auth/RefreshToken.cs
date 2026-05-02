namespace Skyress.Domain.Aggregates.Auth;

public class RefreshToken
{
	public long Id { get; set; }
	public string Token { get; set; } = string.Empty;
	public long UserId { get; set; }
	public Guid FamilyId { get; set; }
	public DateTime ExpiresAt { get; set; }
	public bool IsUsed { get; set; }
	public bool IsRevoked { get; set; }
	public string? ReplacedByToken { get; set; }
	public DateTime CreatedAt { get; set; }
	public User? User { get; set; }

	public bool IsValid => !IsUsed && !IsRevoked && ExpiresAt > DateTime.UtcNow;

	public RefreshToken()
	{
	}

	public RefreshToken(string token, long userId, Guid familyId, DateTime expiresAt)
	{
		Token = token;
		UserId = userId;
		FamilyId = familyId;
		ExpiresAt = expiresAt;
		IsUsed = false;
		IsRevoked = false;
		CreatedAt = DateTime.UtcNow;
	}
}
