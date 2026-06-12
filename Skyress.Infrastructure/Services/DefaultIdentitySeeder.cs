using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Skyress.Application.Auth.Constants;
using Skyress.Application.Auth.Contracts.Services;
using Skyress.Domain.Aggregates.Auth;
using Skyress.Infrastructure.Persistence;

namespace Skyress.Infrastructure.Services;

public static class DefaultIdentitySeeder
{
	public static async Task SeedDefaultIdentityAsync(
		this IServiceProvider serviceProvider,
		IConfiguration configuration,
		CancellationToken cancellationToken = default)
	{
		using var scope = serviceProvider.CreateScope();
		var context = scope.ServiceProvider.GetRequiredService<SkyressDbContext>();
		var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

		await EnsureRoleAsync(context, AuthConstants.RoleIds.Admin, AuthConstants.Roles.Admin, cancellationToken);
		await EnsureRoleAsync(context, AuthConstants.RoleIds.Customer, AuthConstants.Roles.Customer, cancellationToken);

		var adminEmail = configuration["DefaultAdmin:Email"];
		var adminPassword = configuration["DefaultAdmin:Password"];

		if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword))
		{
			await BackfillCustomerRoleAsync(context, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);
			return;
		}

		var admin = await context.Users
			.Include(user => user.UserRoles)
			.FirstOrDefaultAsync(user => user.Email == adminEmail, cancellationToken);

		if (admin is null)
		{
			admin = new User(adminEmail, passwordHasher.HashPassword(adminPassword));
			admin.UserRoles.Add(new UserRole
			{
				User = admin,
				RoleId = AuthConstants.RoleIds.Admin
			});

			await context.Users.AddAsync(admin, cancellationToken);
			await BackfillCustomerRoleAsync(context, cancellationToken);
			await context.SaveChangesAsync(cancellationToken);
			return;
		}

		admin.IsActive = true;

		if (admin.UserRoles.All(userRole => userRole.RoleId != AuthConstants.RoleIds.Admin))
		{
			admin.UserRoles.Add(new UserRole
			{
				UserId = admin.Id,
				RoleId = AuthConstants.RoleIds.Admin
			});
		}

		await BackfillCustomerRoleAsync(context, cancellationToken);
		await context.SaveChangesAsync(cancellationToken);
	}

	private static async Task EnsureRoleAsync(
		SkyressDbContext context,
		int roleId,
		string roleName,
		CancellationToken cancellationToken)
	{
		var role = await context.Roles.FindAsync([roleId], cancellationToken);
		if (role is null)
		{
			await context.Roles.AddAsync(new Role(roleId, roleName), cancellationToken);
			return;
		}

		role.Name = roleName;
	}

	private static async Task BackfillCustomerRoleAsync(
		SkyressDbContext context,
		CancellationToken cancellationToken)
	{
		var usersWithoutRoles = await context.Users
			.Include(user => user.UserRoles)
			.Where(user => !user.UserRoles.Any())
			.ToListAsync(cancellationToken);

		foreach (var user in usersWithoutRoles)
		{
			user.UserRoles.Add(new UserRole
			{
				UserId = user.Id,
				RoleId = AuthConstants.RoleIds.Customer
			});
		}
	}
}
