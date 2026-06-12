using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Auth.Contracts.Persistence;
using Skyress.Domain.Common;

namespace Skyress.Application.Auth.Commands.Logout;

public class LogoutCommandHandler(
	IUserRepository userRepository
) : ICommandHandler<LogoutCommand>
{
	public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
	{
		var refreshToken = await userRepository.GetRefreshTokenAsync(request.RefreshToken, cancellationToken);

		if (refreshToken is { IsRevoked: false })
		{
			refreshToken.Revoke();
			await userRepository.UpdateRefreshTokenAsync(refreshToken, cancellationToken);
		}

		return Result.Success();
	}
}
