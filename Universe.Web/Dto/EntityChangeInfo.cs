namespace Universe.Web.Dto;

public enum EntityChangeType
{
	Created = 0,
	Deleted = 1,
	Modified = 2,
}

public class EntityChangeInfo
{
	public EntityChangeType ChangeType { get; set; }

	public string EntityTypeName { get; set; } = string.Empty;

	public dynamic? FullNewValue { get; set; }

	public string? PrimaryKeyFieldName { get; set; }

	public dynamic? PrimaryKeyValue { get; set; }

	public IDictionary<string, dynamic>? FieldChanges { get; set; }
}
