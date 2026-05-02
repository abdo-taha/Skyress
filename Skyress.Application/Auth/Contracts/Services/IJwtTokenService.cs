using Skyress.Domain.Aggregates.Auth;

namespace Skyress.Application.Auth.Contracts.Services;

public interface IJwtTokenService
{
	int AccessTokenExpirySeconds { get; }
	int RefreshTokenExpiryDays { get; }
	string GenerateAccessToken(User user, List<string> roles);
	string GenerateRefreshToken();
	(string AccessToken, string RefreshTokenValue, RefreshToken RefreshTokenEntity) CreateTokenPair(User user, IList<string> roles, Guid? familyId = null);
}
