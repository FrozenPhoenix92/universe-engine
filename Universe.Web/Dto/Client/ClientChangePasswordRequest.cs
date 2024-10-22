namespace Universe.Web.Dto.Client;

public class ClientChangePasswordRequest
{
	public Guid ClientId { get; set; }

	public string OldPassword { get; set; } = string.Empty;

	public string NewPassword { get; set; } = string.Empty;
}
