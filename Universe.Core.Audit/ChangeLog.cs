using Universe.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Universe.Core.Audit;

public class ChangeLog : IEntity
{
	public IEnumerable<ChangeLogItem> ChangeLogItems { get; set; } = new List<ChangeLogItem>();

	public DateTimeOffset DateTime { get; set; }

	public int EntityId { get; set; }

	public string EntityName { get; set; } = string.Empty;

	public int Id { get; set; }

    public EntityState State { get; set; }

    public string TableName { get; set; } = string.Empty;

    public string? UserName { get; set; }
}
