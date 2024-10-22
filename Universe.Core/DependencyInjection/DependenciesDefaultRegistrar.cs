using System.Text.RegularExpressions;

using Universe.Core.ModuleBinding;

using Microsoft.Extensions.DependencyInjection;

namespace Universe.Core.DependencyInjection;

/// <summary>
/// Класс, выполняющий регистрацию сервисов в механизме внедрения зависимостей по умолчанию.
/// </summary>
public static class DependenciesDefaultRegistrar
{
	private static readonly Regex serviceTypeNameRegex = new(@"^I(.+?)Service$");

	/// <summary>
	/// Создаёт типы сервиса данных с конкретными универсальными параметрами
	/// </summary>
	public static void AddDataServiceDependencies()
	{

	}

	/// <summary>
	/// Для сервисов, имена которых соответствуют именам их интерфейсов, и для которых не указаны соответствия явно,
	/// добавляет в контейнер зависимости по умолчанию.
	/// Таким образом, нет необходимости указывать зависимости вида ISomeService -> SomeService.
	/// Достаточно создать классы с именами формата "I<ServiceName>Service" и "<ServiceName>Service" соответственно.
	/// </summary>
	public static void AddDefaultServicesDependencies(IServiceCollection services)
	{
		var allProjectTypes = AssembliesManager.GetProjectAssemblies().SelectMany(x => x.GetTypes());
		var unregisteredServicesTypes = allProjectTypes
			.Where(x => x.IsPublic &&
						x.IsInterface &&
						!x.IsGenericType &&
						serviceTypeNameRegex.IsMatch(x.Name) &&
						services.All(y => y.ServiceType.Name != x.Name));

		foreach (var unregisteredService in unregisteredServicesTypes)
		{
			var implementation = allProjectTypes
				.SingleOrDefault(x => x.IsClass &&
									  !x.IsGenericType &&
									  x.Name.Equals(unregisteredService.Name.Substring(1), StringComparison.InvariantCultureIgnoreCase) &&
									  GetDirectInterfaces(x).Contains(unregisteredService));

			if (implementation is null) continue;

			services.AddScoped(unregisteredService, implementation);
		}
	}


	private static Type[] GetDirectInterfaces(Type type)
	{
		var implementedInterfaces = type.GetInterfaces();

		return implementedInterfaces
			.Except(implementedInterfaces.SelectMany(type => type.GetInterfaces()))
			.ToArray();
	}
}
