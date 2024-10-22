using Universe.Core.Data;

namespace Universe.Files.Model;

public class FileDetails : IEntity
{
    public DateTimeOffset Created { get; set; }

    public string? Extension { get; set; }

    public int Id { get; set; }

    public DateTimeOffset LatestUpdated { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? RelatedDataKey { get; set; }

    public string RelatedOperationKey { get; set; } = string.Empty;

    public string? RelatedResponsibleKey { get; set; }
}