using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Auth.Contracts.Persistence;
using Skyress.Application.Auth.Contracts.Services;
using Skyress.Application.Auth.DTOs;
using Skyress.Domain.Common;

namespace Skyress.Application.Auth.Commands.Login;

public class LoginCommandHandler(
	IUserRepository userRepository,
	IPasswordHasher passwordHasher,
	IJwtTokenService jwtTokenService
) : ICommandHandler<LoginCommand, TokenResponse>
{
	public async Task<Result<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
	{
		var user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);
		if (user == null)
			throw new UnauthorizedAccessException("Invalid email or password");

		var passwordValid = passwordHasher.VerifyPassword(request.Password, user.PasswordHash);
		if (!passwordValid)
			throw new UnauthorizedAccessException("Invalid email or password");

		if (!user.IsActive)
			throw new InvalidOperationException("User account is inactive");

		var roles = user.UserRoles
			.Where(ur => ur.Role != null)
			.Select(ur => ur.Role!.Name)
			.ToList();

		var (accessToken, refreshTokenValue, refreshTokenEntity) = jwtTokenService.CreateTokenPair(user, roles);

		user.RefreshTokens.Add(refreshTokenEntity);
		await userRepository.SaveChangesAsync(cancellationToken);

		return new TokenResponse(accessToken, refreshTokenValue, jwtTokenService.AccessTokenExpirySeconds);
	}
}
