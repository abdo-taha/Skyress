namespace Skyress.Application.Auth.DTOs;

public record TokenResponse(
	string AccessToken,
	string RefreshToken,
	int ExpiresIn,
	string TokenType = "Bearer"
);
