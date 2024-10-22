using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Universe.Core.ModuleBinding.Bindings;

/// <summary>
/// Интерфейс привязки модуля к проекту, дающий возможность добавления зависимостей.
/// </summary>
/// <remarks>
/// Один из интерфейсов, используемых модулями для мягкой привязки к проекту. В момент старта приложения классы, реализующие такой интерфейс,
/// будут ипользованы через рефлексию для настройки модуля. При этом в коде основного проекта нет прямого использование классов модулей и,
/// как следствие, ссылок на пространства имён, что позволяет отвязать/привязать модулм от проекта простым удалением/добавлением в решение.
/// </remarks>
public interface IDependenciesModuleBinding
{
    /// <summary>
    /// Добавляет зависимости модуля.
    /// </summary>
    void AddDependencies(IServiceCollection services, IConfiguration configuration);
}