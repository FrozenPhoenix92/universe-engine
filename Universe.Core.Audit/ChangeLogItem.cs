using Universe.Core.Data;

namespace Universe.Core.Audit;

public class ChangeLogItem : IEntity
{
    public int Id { get; set; }

    public string PropertyName { get; set; } = string.Empty;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }


    public int ChangeLogId { get; set; }

    public ChangeLog? ChangeLog { get; set; }
}
