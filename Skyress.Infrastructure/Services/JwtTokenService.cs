using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Skyress.Application.Auth.Contracts.Services;
using Skyress.Domain.Aggregates.Auth;

namespace Skyress.Infrastructure.Services;

public class JwtTokenService(IOptions<JwtSettings> options) : IJwtTokenService
{
	private readonly JwtSettings _settings = options.Value;

	public int AccessTokenExpirySeconds => _settings.AccessTokenExpiryMinutes * 60;
	public int RefreshTokenExpiryDays => _settings.RefreshTokenExpiryDays;

	public string GenerateAccessToken(User user, List<string> roles)
	{
		if (string.IsNullOrEmpty(_settings.SecretKey))
			throw new InvalidOperationException("JWT:SecretKey is not configured");

		var tokenHandler = new JwtSecurityTokenHandler();
		var key = Encoding.ASCII.GetBytes(_settings.SecretKey);

		var claims = new List<Claim>
		{
			new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
			new(JwtRegisteredClaimNames.Email, user.Email),
			new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
		};

		foreach (var role in roles)
		{
			claims.Add(new Claim(ClaimTypes.Role, role));
		}

		var tokenDescriptor = new SecurityTokenDescriptor
		{
			Subject = new ClaimsIdentity(claims),
			Expires = DateTime.UtcNow.AddMinutes(_settings.AccessTokenExpiryMinutes),
			Issuer = _settings.Issuer,
			Audience = _settings.Audience,
			SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
		};

		var token = tokenHandler.CreateToken(tokenDescriptor);
		return tokenHandler.WriteToken(token);
	}

	public string GenerateRefreshToken()
	{
		var randomNumber = new byte[64];
		using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
		rng.GetBytes(randomNumber);
		return Convert.ToBase64String(randomNumber);
	}

	public (string AccessToken, string RefreshTokenValue, RefreshToken RefreshTokenEntity) CreateTokenPair(
		User user, IList<string> roles, Guid? familyId = null)
	{
		var accessToken = GenerateAccessToken(user, [.. roles]);
		var refreshTokenValue = GenerateRefreshToken();
		var refreshTokenEntity = new RefreshToken(
			refreshTokenValue,
			user.Id,
			familyId ?? Guid.NewGuid(),
			DateTime.UtcNow.AddDays(_settings.RefreshTokenExpiryDays)
		);
		return (accessToken, refreshTokenValue, refreshTokenEntity);
	}
}
