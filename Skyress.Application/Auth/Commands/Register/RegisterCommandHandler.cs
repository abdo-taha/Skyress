using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Auth.Constants;
using Skyress.Application.Auth.Contracts.Persistence;
using Skyress.Application.Auth.Contracts.Services;
using Skyress.Application.Auth.DTOs;
using Skyress.Domain.Aggregates.Auth;
using Skyress.Domain.Common;

namespace Skyress.Application.Auth.Commands.Register;

public class RegisterCommandHandler(
	IUserRepository userRepository,
	IPasswordHasher passwordHasher,
	IJwtTokenService jwtTokenService
) : ICommandHandler<RegisterCommand, TokenResponse>
{
	public async Task<Result<TokenResponse>> Handle(RegisterCommand request, CancellationToken cancellationToken)
	{
		var emailExists = await userRepository.ExistsByEmailAsync(request.Email, cancellationToken);
		if (emailExists)
			throw new InvalidOperationException($"User with email {request.Email} already exists");

		var passwordHash = passwordHasher.HashPassword(request.Password);
		var user = new User(request.Email, passwordHash);
		user.UserRoles.Add(new UserRole
		{
			User = user,
			RoleId = AuthConstants.RoleIds.Customer
		});

		await userRepository.AddAsync(user, cancellationToken);

		var roles = new List<string> { AuthConstants.Roles.Customer };
		var (accessToken, refreshTokenValue, refreshTokenEntity) = jwtTokenService.CreateTokenPair(user, roles);

		user.RefreshTokens.Add(refreshTokenEntity);
		await userRepository.SaveChangesAsync(cancellationToken);

		return new TokenResponse(accessToken, refreshTokenValue, jwtTokenService.AccessTokenExpirySeconds);
	}
}
