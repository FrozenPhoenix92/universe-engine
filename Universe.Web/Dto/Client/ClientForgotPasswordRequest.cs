namespace Universe.Web.Dto.Client;

public class ClientForgotPasswordRequest
{
	public string CaptchaResponse { get; set; } = string.Empty;

	public string Email { get; set; } = string.Empty;
}
