namespace Skyress.Infrastructure.Services;

public class JwtSettings
{
	public string Issuer { get; set; } = "Skyress";
	public string Audience { get; set; } = "SkyressAPI";
	public int AccessTokenExpiryMinutes { get; set; } = 15;
	public int RefreshTokenExpiryDays { get; set; } = 7;
	public string SecretKey { get; set; } = string.Empty;
}
