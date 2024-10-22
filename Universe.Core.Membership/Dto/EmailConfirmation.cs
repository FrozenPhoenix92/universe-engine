namespace Universe.Core.Membership.Dto;

public class EmailConfirmation
{
	public Guid? UserId { get; set; }

	public string? Token { get; set; }
}
