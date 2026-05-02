using Skyress.Domain.Aggregates.Auth;

namespace Skyress.Application.Auth.Contracts.Persistence;

public interface IUserRepository
{
	Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task<User?> GetByIdAsync(long userId, CancellationToken cancellationToken = default);
	Task AddAsync(User user, CancellationToken cancellationToken = default);
	Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default);
	Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
	Task<List<RefreshToken>> GetRefreshTokensByFamilyIdAsync(Guid familyId, CancellationToken cancellationToken = default);
	Task UpdateRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken = default);
	Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
