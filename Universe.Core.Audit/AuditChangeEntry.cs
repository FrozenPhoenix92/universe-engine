using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Universe.Core.Audit;

public class AuditChangeEntry
{
    public AuditChangeEntry(EntityEntry entry) => State = entry.State;


    public EntityState State { get; set; }

    public string TableName { get; set; }

    public string EntityName { get; set; }

    public string UserName { get; set; }

    public int EntityId { get; set; }

    public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();

    public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();

    public List<PropertyEntry> KeyProperties { get; } = new List<PropertyEntry>();


    public ChangeLog ToAudit() => new()
    {
        State = State,
        UserName = UserName,
        TableName = TableName,
        EntityName = EntityName,
        DateTime = DateTime.UtcNow,
        EntityId = EntityId,
        ChangeLogItems = NewValues.Keys.Select(k => new ChangeLogItem
        {
            NewValue = NewValues[k]?.ToString(),
            OldValue = OldValues.ContainsKey(k) && OldValues[k] is not null ? OldValues[k].ToString() : null,
            PropertyName = k
        }).ToArray()
    };
}
