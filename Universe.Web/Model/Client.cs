using Universe.Core.Data;

namespace Universe.Web.Model;

public class Client : IEntity<Guid>
{
	public bool AccountConfirmed { get; set; }

	public string AppCode { get; set; } = string.Empty;

	public bool Blocked { get; set; }

	public string Email { get; set; } = string.Empty;

	public Guid Id { get; set; }

	public string? PasswordHash { get; set; }

	public string? PasswordSalt { get; set; }

	public DateTime SignUpDate { get; set; }
}
