using Universe.Core.AppConfiguration.Model;
using Universe.Core.Membership;

namespace Universe.Core.Common.Model;

/// <summary>
/// Представляет набор найстроек конфигурации системы.
/// </summary>
public class AppSettingsSet : AppSettingsSetCore
{
	public AggregatedRole? AggregatedRole { get; set; }


	/// <summary>
	/// Набор ролей, имеющих доступ к данному набору настроек.
	/// </summary>
	public IList<AppSettingsSetRole> Roles { get; set; } = new List<AppSettingsSetRole>();

	/// <summary>
	/// Набор разрешений, доющих доступ к данному набору настроек.
	/// </summary>
	public IList<AppSettingsSetPermission> Permissions { get; set; } = new List<AppSettingsSetPermission>();
}
