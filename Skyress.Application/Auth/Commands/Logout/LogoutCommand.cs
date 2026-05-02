using Skyress.Application.Abstractions.Messaging;

namespace Skyress.Application.Auth.Commands.Logout;

public record LogoutCommand(
	string RefreshToken
) : ICommand;
