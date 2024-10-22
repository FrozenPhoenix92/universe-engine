namespace Universe.Core.AppConfiguration.Configuration;

/// <summary>
/// Представляет общую информацию о приложении.
/// </summary>
public class AppSettings
{
	/// <summary>
	/// Описание приложения.
	/// </summary>
	public string? Description { get; set; }

	/// <summary>
	/// Название приложения.
	/// </summary>
	public string? Name { get; set; }

	/// <summary>
	/// Короткая версия названия приложения.
	/// </summary>
	public string? ShortName { get; set; }
}
