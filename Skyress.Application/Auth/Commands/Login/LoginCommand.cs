using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Auth.DTOs;

namespace Skyress.Application.Auth.Commands.Login;

public record LoginCommand(
	string Email,
	string Password
) : ICommand<TokenResponse>;