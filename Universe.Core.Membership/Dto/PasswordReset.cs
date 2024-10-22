namespace Universe.Core.Membership.Dto;

public class PasswordReset
{
	public string? Password { get; set; }

	public string? Token { get; set; }

	public Guid UserId { get; set; }
}
