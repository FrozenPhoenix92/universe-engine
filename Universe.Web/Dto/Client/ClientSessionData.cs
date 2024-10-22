namespace Universe.Web.Dto.Client; 

public class ClientSessionData
{
	public bool AccountConfirmed { get; set; }

	public bool Blocked { get; set; }

	public string Email { get; set; } = string.Empty;

	public Guid Id { get; set; }
}
