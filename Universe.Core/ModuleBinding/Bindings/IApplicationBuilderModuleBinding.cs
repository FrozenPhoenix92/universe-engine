using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Universe.Core.ModuleBinding.Bindings;

/// <summary>
/// Интерфейс привязки модуля к проекту, дающий возможность настройки конфигурации приложения.
/// </summary>
/// <remarks>
/// Один из интерфейсов, используемых модулями для мягкой привязки к проекту. В момент старта приложения классы, реализующие такой интерфейс,
/// будут ипользованы через рефлексию для настройки модуля. При этом в коде основного проекта нет прямого использование классов модулей и,
/// как следствие, ссылок на пространства имён, что позволяет отвязать/привязать модулм от проекта простым удалением/добавлением в решение.
/// </remarks>
public interface IApplicationBuilderModuleBinding
{
	/// <summary>
	/// Настраивает конфигурацию приложения для модуля.
	/// </summary>
	/// <param name="app">Объект для настройки конфигурации приложения.</param>
	/// <param name="configuration">Объект, содержащий конфигурацию системы.</param>
	void ConfigureModule(IApplicationBuilder app, IConfiguration configuration);
}
