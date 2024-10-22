using Universe.Core.AppConfiguration.Model;
using Universe.Core.Data;

namespace Universe.Core.AppConfiguration.Services;

public interface IAppSettingsService
{
    T? GetSettings<T>() where T : class;

	bool HasSettings<T>() where T : class;

	/// <summary>
	/// Выполняет загрузку всех наборов настроек из хранилища, преобразует в соответствующие им типы и записывает их в словарь.
	/// Подобная операция должна проводиться единожды после того, как хранилище будет содержать все необходимые наборы настроек.
	/// Добавление новых наборов после инициализации невозможно.
	/// </summary>
	Task Initialize<TAppSettingsModel>(IDbContext context, IServiceProvider serviceProvider) where TAppSettingsModel : AppSettingsSetCore;

	Task ReplaceSettings(Type key, object value, IServiceProvider? serviceProvider);
}
