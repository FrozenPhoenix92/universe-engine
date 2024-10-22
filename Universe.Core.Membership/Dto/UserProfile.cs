namespace Universe.Core.Membership.Dto;

public class UserProfilePersonal
{
	public string? Email { get; set; }

	public string? FirstName { get; set; }

	public string? LastName { get; set; }

	public IEnumerable<string> Permissions { get; set; } = new List<string>();

	public string? PhoneNumber { get; set; }

	public IEnumerable<string> Roles { get; set; } = new List<string>();
}
