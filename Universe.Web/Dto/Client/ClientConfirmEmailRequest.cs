namespace Universe.Web.Dto.Client;

public class ClientConfirmEmailRequest
{
	public string CaptchaResponse { get; set; } = string.Empty;

	public string Password { get; set; } = string.Empty;

	public string Token { get; set; } = string.Empty;

	public Guid UserId { get; set; }
}
