using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Auth.DTOs;

namespace Skyress.Application.Auth.Commands.Register;

public record RegisterCommand(
	string Email,
	string Password
) : ICommand<TokenResponse>;
