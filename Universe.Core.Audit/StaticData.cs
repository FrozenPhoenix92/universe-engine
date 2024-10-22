using Universe.Core.Configuration;

namespace Universe.Core.Audit;

public static class StaticData
{
	public const string AuditDataContextBindingId = "AuditDataContext";
	public const string DatabaseConnectionSettingsConfigurationSectionName = $"Audit{nameof(DatabaseConnectionSettings)}";
}
