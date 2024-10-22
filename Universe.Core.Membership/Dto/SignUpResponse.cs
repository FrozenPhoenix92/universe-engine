namespace Universe.Core.Membership.Dto;

public class SignUpResponse
{
	public bool ApprovalRequired { get; set; }

	public bool EmailConfirmationRequired { get; set; }

	public bool PhoneNumberConfirmationRequired { get; set; }

	public SignInResponse? SignInResponse { get; set; }
}
