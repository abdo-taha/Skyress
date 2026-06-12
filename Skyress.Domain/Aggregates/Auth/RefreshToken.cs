using System.Security.Cryptography;
using System.Text;

namespace Skyress.Domain.Aggregates.Auth;

public class RefreshToken
{
	public long Id { get; set; }
	public string Token { get; set; } = string.Empty;
	public string TokenHash { get; set; } = string.Empty;
	public long UserId { get; set; }
	public Guid FamilyId { get; set; }
	public DateTime ExpiresAt { get; set; }
	public bool IsUsed { get; set; }
	public bool IsRevoked { get; set; }
	public string? ReplacedByToken { get; set; }
	public DateTime CreatedAt { get; set; }
	public byte[] RowVersion { get; set; } = [];
	public User? User { get; set; }

	public bool IsValid => !IsUsed && !IsRevoked && ExpiresAt > DateTime.UtcNow;

	public RefreshToken()
	{
	}

	public RefreshToken(string token, long userId, Guid familyId, DateTime expiresAt)
	{
		Token = string.Empty;
		TokenHash = Hash(token);
		UserId = userId;
		FamilyId = familyId;
		ExpiresAt = expiresAt;
		IsUsed = false;
		IsRevoked = false;
		CreatedAt = DateTime.UtcNow;
	}

	public static RefreshToken FromHash(string tokenHash, long userId, Guid familyId, DateTime expiresAt)
	{
		return new RefreshToken
		{
			Token = string.Empty,
			TokenHash = tokenHash,
			UserId = userId,
			FamilyId = familyId,
			ExpiresAt = expiresAt,
			IsUsed = false,
			IsRevoked = false,
			CreatedAt = DateTime.UtcNow
		};
	}

	public static string Hash(string token)
	{
		var tokenBytes = Encoding.UTF8.GetBytes(token);
		var hashBytes = SHA256.HashData(tokenBytes);
		return Convert.ToHexString(hashBytes);
	}

	public void MarkUsed(string replacementToken)
	{
		IsUsed = true;
		ReplacedByToken = Hash(replacementToken);
	}

	public void Revoke()
	{
		IsRevoked = true;
	}
}
