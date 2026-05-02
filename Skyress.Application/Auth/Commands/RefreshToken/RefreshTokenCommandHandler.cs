using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Auth.Contracts.Persistence;
using Skyress.Application.Auth.Contracts.Services;
using Skyress.Application.Auth.DTOs;
using Skyress.Domain.Common;

namespace Skyress.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler(
	IUserRepository userRepository,
	IJwtTokenService jwtTokenService
) : ICommandHandler<RefreshTokenCommand, TokenResponse>
{
	public async Task<Result<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
	{
		var refreshToken = await userRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);

		if (refreshToken == null)
			throw new UnauthorizedAccessException("TOKEN_NOT_FOUND");

		if (refreshToken.IsRevoked)
			throw new UnauthorizedAccessException("TOKEN_REVOKED");

		if (refreshToken.IsUsed)
		{
			var family = await userRepository.GetRefreshTokensByFamilyIdAsync(refreshToken.FamilyId, cancellationToken);
			foreach (var token in family)
			{
				token.IsRevoked = true;
			}
			foreach (var token in family)
			{
				await userRepository.UpdateRefreshTokenAsync(token, cancellationToken);
			}
			throw new UnauthorizedAccessException("TOKEN_ALREADY_USED");
		}

		if (refreshToken.ExpiresAt <= DateTime.UtcNow)
			throw new UnauthorizedAccessException("TOKEN_EXPIRED");

		var user = await userRepository.GetByIdAsync(refreshToken.UserId, cancellationToken);
		if (user == null || !user.IsActive)
			throw new UnauthorizedAccessException("User not found or inactive");

		var roles = user.UserRoles
			.Where(ur => ur.Role != null)
			.Select(ur => ur.Role!.Name)
			.ToList();

		var (newAccessToken, newRefreshTokenValue, newRefreshTokenEntity) =
			jwtTokenService.CreateTokenPair(user, roles, refreshToken.FamilyId);

		refreshToken.IsUsed = true;
		refreshToken.ReplacedByToken = newRefreshTokenValue;
		await userRepository.UpdateRefreshTokenAsync(refreshToken, cancellationToken);

		user.RefreshTokens.Add(newRefreshTokenEntity);
		await userRepository.SaveChangesAsync(cancellationToken);

		return new TokenResponse(newAccessToken, newRefreshTokenValue, jwtTokenService.AccessTokenExpirySeconds);
	}
}
