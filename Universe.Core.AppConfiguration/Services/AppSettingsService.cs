using System.Text.Json;

using Universe.Core.Data;
using Universe.Core.Exceptions;
using Universe.Core.JsonSerialization;
using Universe.Core.ModuleBinding;
using Universe.Core.AppConfiguration.Model;

namespace Universe.Core.AppConfiguration.Services;

/// <inheritdoc cref="IAppSettingsService"/>
public class AppSettingsService : IAppSettingsService
{
	private IDictionary<Type, object> _settings = new Dictionary<Type, object>();


	public T? GetSettings<T>() where T : class
	{
		var type = typeof(T);

		if (!_settings.ContainsKey(type)) return null;

		if (_settings[type] is not T settingsSetValue)
			throw new ConflictException("Сервис настроек конфигурации системы содержит набор, данные которого не могут быть " +
				"преобразованы в объект своего типа.");

		return settingsSetValue;
	}

	public bool HasSettings<T>() where T : class => _settings.ContainsKey(typeof(T));

	public async Task Initialize<TAppSettingsModel>(IDbContext context, IServiceProvider serviceProvider)
		where TAppSettingsModel : AppSettingsSetCore
	{
		foreach (var settingsSet in context.Set<TAppSettingsModel>().ToList())
		{
			var configType = AssembliesManager.GetProjectAssemblies()
				.SelectMany(x => x.GetTypes())
				.SingleOrDefault(x => x.Name == settingsSet.Name);

			if (configType is null)
			{
				context.Set<TAppSettingsModel>().Remove(settingsSet);
			}
			else
			{
				var settingsSetValue = JsonSerializer.Deserialize(settingsSet.Value ?? "{}", configType, JsonSerializerOptionsProvider.GetOptions());

				if (settingsSetValue is null)
					throw new ConflictException($"Хранилище данных конфигурации системы содержит набор настроек '{settingsSet.Name}', " +
						$"который не соответствует своему типу данных. Возможно, тип данных настроек был изменён, " +
						$"либо в следствие изменения настроек внешним способом в обход логики валидации данные стали некорректными.");

				await ReplaceSettings(configType, settingsSetValue, serviceProvider);
			}
		}

		context.SaveChanges();
	}

	public async Task ReplaceSettings(Type key, object value, IServiceProvider? serviceProvider)
	{
		_settings[key] = value;

		if (value is IAppSettingsOnChangeCallback settings && serviceProvider is not null)
			await settings.OnChange(serviceProvider);
	}
}