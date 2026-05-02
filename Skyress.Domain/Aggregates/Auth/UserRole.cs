namespace Skyress.Domain.Aggregates.Auth;

public class UserRole
{
	public long UserId { get; set; }
	public int RoleId { get; set; }
	public User? User { get; set; }
	public Role? Role { get; set; }

	public UserRole()
	{
	}

	public UserRole(long userId, int roleId)
	{
		UserId = userId;
		RoleId = roleId;
	}
}
