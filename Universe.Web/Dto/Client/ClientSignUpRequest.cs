namespace Universe.Web.Dto.Client;

public class ClientSignUpRequest
{
	public string? AppCode { get; set; }

	public string CaptchaResponse { get; set; } = string.Empty;

	public string? Email { get; set; }

	public string Password { get; set; } = string.Empty;

	public string? Token { get; set; }

	public Guid? UserId { get; set; }
}