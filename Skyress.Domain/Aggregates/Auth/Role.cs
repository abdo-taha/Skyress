namespace Skyress.Domain.Aggregates.Auth;

public class Role
{
	public int Id { get; set; }
	public string Name { get; set; } = string.Empty;
	public ICollection<UserRole> UserRoles { get; set; } = [];

	public Role()
	{
	}

	public Role(int id, string name)
	{
		Id = id;
		Name = name;
	}
}
