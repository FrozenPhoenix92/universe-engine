namespace Universe.Core.AppConfiguration;

/// <summary>
/// Представляет набор настроек, требующий выполнение действия в ответ на изменение данных.
/// </summary>
public interface IAppSettingsOnChangeCallback
{
	/// <summary>
	/// Функция, вызывающаяся после изменения данных набора настроек.
	/// </summary>
	Task OnChange(IServiceProvider serviceProvider, CancellationToken ct = default);
}
