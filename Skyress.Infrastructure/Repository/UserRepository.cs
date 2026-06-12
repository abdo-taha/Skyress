using Microsoft.EntityFrameworkCore;
using Skyress.Application.Auth.Contracts.Persistence;
using Skyress.Domain.Aggregates.Auth;
using Skyress.Infrastructure.Persistence;

namespace Skyress.Infrastructure.Repository;

public class UserRepository : GenericRepository<User>, IUserRepository
{
	public UserRepository(SkyressDbContext context) : base(context) { }

	public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		return await SkyressDbContext.Users
			.Include(u => u.UserRoles)
			.ThenInclude(ur => ur.Role)
			.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
	}

	public new async Task<User?> GetByIdAsync(long userId, CancellationToken cancellationToken = default)
	{
		return await SkyressDbContext.Users
			.Include(u => u.UserRoles)
			.ThenInclude(ur => ur.Role)
			.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
	}

	public async Task AddAsync(User user, CancellationToken cancellationToken = default)
	{
		await DbSet.AddAsync(user, cancellationToken);
		await SaveChangesAsync(cancellationToken);
	}

	public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken = default)
	{
		return await SkyressDbContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
	}

	public async Task<RefreshToken?> GetRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
	{
		var tokenHash = RefreshToken.Hash(token);
		return await SkyressDbContext.RefreshTokens
			.Include(rt => rt.User)
			.FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash, cancellationToken);
	}

	public async Task<List<RefreshToken>> GetRefreshTokensByFamilyIdAsync(Guid familyId, CancellationToken cancellationToken = default)
	{
		return await SkyressDbContext.RefreshTokens
			.Where(rt => rt.FamilyId == familyId)
			.ToListAsync(cancellationToken);
	}

	public async Task UpdateRefreshTokenAsync(RefreshToken token, CancellationToken cancellationToken = default)
	{
		SkyressDbContext.RefreshTokens.Update(token);
		await SaveChangesAsync(cancellationToken);
	}

	public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		await SkyressDbContext.SaveChangesAsync(cancellationToken);
	}
}
