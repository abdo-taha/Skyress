using Skyress.Application.Abstractions.Messaging;
using Skyress.Application.Auth.DTOs;

namespace Skyress.Application.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(
	string RefreshToken
) : ICommand<TokenResponse>;
