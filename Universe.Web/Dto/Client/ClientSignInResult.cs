namespace Universe.Web.Dto.Client;

public class ClientSignInResult
{
	public string AccessToken { get; set; } = string.Empty;

	public bool EmailConfirmationRequired { get; set; }

	public ClientSessionData? SessionData { get; set; }
}
