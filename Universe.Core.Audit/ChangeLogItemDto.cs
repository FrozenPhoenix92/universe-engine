using Universe.Core.Infrastructure;

namespace Universe.Core.Audit;

public class ChangeLogItemDto : IDto
{
    public int Id { get; set; }

    public string PropertyName { get; set; } = string.Empty;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }
}
