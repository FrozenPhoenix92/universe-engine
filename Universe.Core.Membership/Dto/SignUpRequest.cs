namespace Universe.Core.Membership.Dto;

public class SignUpRequest
{
	public string? Email { get; set; }

	public string? FirstName { get; set; }

	public string? LastName { get; set; }

	public string? Password { get; set; }

	public string UserName { get; set; } = string.Empty;
}
