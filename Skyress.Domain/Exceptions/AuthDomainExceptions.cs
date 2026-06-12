namespace Skyress.Domain.Exceptions;

public sealed class RefreshTokenInvalidException(string message)
    : DomainException("RefreshToken.Invalid", message);

public sealed class RefreshTokenReplayDetectedException()
    : DomainException("RefreshToken.ReplayDetected", "Refresh token replay detected.");
