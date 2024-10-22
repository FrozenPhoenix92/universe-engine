using Universe.Core.Membership.Model;

namespace Universe.Core.Common.Model;

/// <summary>
/// Представляет связь между набором настроек и ролью, имеющей доступ к этому набору.
/// </summary>
public class AppSettingsSetRole
{
	/// <summary>
	/// Идентификатор роли, имеющей доступ к данному набору настроек.
	/// </summary>
	public Guid RoleId { get; set; }

	/// <summary>
	/// Разрешение, имеющая доступ к данному набору настроек.
	/// </summary>
	public Role? Role { get; set; }

	/// <summary>
	/// Идентификатор набора настроек.
	/// </summary>
	public int AppSettingsSetId { get; set; }

	/// <summary>
	/// Набор настроек.
	/// </summary>
	public AppSettingsSet? AppSettingsSet { get; set; }

	/// <summary>
	/// Определяет, даёт ли роль возможность изменять набор настроек.
	/// </summary>
	public bool AllowChange { get; set; } = false;
}
