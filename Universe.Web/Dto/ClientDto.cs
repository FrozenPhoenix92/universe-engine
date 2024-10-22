using Universe.Core.Infrastructure;

namespace Universe.Web.Dto;

public class ClientDto : IDto<Guid>
{
	public bool AccountConfirmed { get; set; }

	public string AppCode { get; set; } = string.Empty;

	public bool Blocked { get; set; }

	public string Email { get; set; } = string.Empty;

	public Guid Id { get; set; }

	public DateTime SignUpDate { get; set; }
}