using Universe.Core.Membership.Model;

namespace Universe.Core.Common.Model;

/// <summary>
/// Представляет связь между набором настроек и разрешениями, дающими доступ к этому набору.
/// </summary>
public class AppSettingsSetPermission
{
	/// <summary>
	/// Идентификатор разрешения, дающего доступ к данному набору настроек.
	/// </summary>
	public Guid PermissionId { get; set; }

	/// <summary>
	/// Разрешение, дающее доступ к данному набору настроек.
	/// </summary>
	public Permission? Permission { get; set; }

	/// <summary>
	/// Идентификатор набора настроек.
	/// </summary>
	public int AppSettingsSetId { get; set; }

	/// <summary>
	/// Набор настроек.
	/// </summary>
	public AppSettingsSet? AppSettingsSet { get; set; }

	/// <summary>
	/// Определяет, даёт ли разрешение возможность изменять набор настроек.
	/// </summary>
	public bool AllowChange { get; set; }
}
