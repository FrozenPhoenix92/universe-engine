namespace Universe.Core.Membership.Authorization;

/// <summary>
/// Интерфейс привязки модуля к проекту, дающий возможность указания разрешений, используемых в модуле, которые необходимо добавить.
/// Используйте строковые константы для обозначения названий прав. Они будут автоматически созданы в процессе инициализации начальных данных проекта.
/// </summary>
/// <remarks>
/// Один из интерфейсов, используемых модулями для мягкой привязки к проекту. В момент старта приложения классы, реализующие такой интерфейс,
/// будут ипользованы через рефлексию для настройки модуля. При этом в коде основного проекта нет прямого использование классов модулей и,
/// как следствие, ссылок на пространства имён, что позволяет отвязать/привязать модулм от проекта простым удалением/добавлением в решение.
/// </remarks>
public interface IPermissionProvider
{
}

public interface IRoleProvider
{

}

public interface IRolePermissionProvider
{
	IDictionary<string, IEnumerable<string>> RolesPermissionsMap { get; }
}

public interface ISuperAdminPermissions : IPermissionProvider
{
}
