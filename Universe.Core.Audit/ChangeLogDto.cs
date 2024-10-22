using Universe.Core.Infrastructure;

using Microsoft.EntityFrameworkCore;

namespace Universe.Core.Audit;

public class ChangeLogDto : IDto
{
	public IEnumerable<ChangeLogItemDto> ChangeLogItems { get; set; } = new List<ChangeLogItemDto>();

	public DateTimeOffset DateTime { get; set; }

	public int EntityId { get; set; }

	public string EntityName { get; set; } = string.Empty;

	public int Id { get; set; }

	public EntityState State { get; set; }

	public string TableName { get; set; } = string.Empty;

	public string? UserName { get; set; }
}
