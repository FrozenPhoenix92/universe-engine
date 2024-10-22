using Universe.Core.Data;

namespace Universe.Web.Model;

public class ClientOperationConfirmationToken : IEntity
{
	public Client? Client { get; set; }

	public Guid ClientId { get; set; }

	public DateTimeOffset? ExpiredAt { get; set; }

	public int Id { get; set; }

	public string Operation { get; set; } = string.Empty;

	public string Token { get; set; } = string.Empty;

	public DateTimeOffset UpdatedAt { get; set; }
}
