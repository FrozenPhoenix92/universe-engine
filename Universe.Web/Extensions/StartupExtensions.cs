using Universe.Core.Mapping;
using Universe.Core.ModuleBinding;
using Universe.Core.ModuleBinding.Bindings;

namespace Universe.Web.Extensions
{
	/// <summary>
	/// Ресширения для процесса старта приложения.
	/// </summary>
	public static class StartupExtensions
	{
		/// <summary>
		/// Добавляет Automapper со всеми настройками сборок и применяет правила по умолчанию для типов, для которых настройки не указаны.
		/// </summary>
		public static void AddObjectsMapping(this IServiceCollection services)
		{
			services.AddAutoMapper(AssembliesManager.GetProjectAssemblies());
			services.AddAutoMapper(DefaultBehaviourProvider.AddDefaultRules);
		}

		/// <summary>
		/// Создаёт базы данных и инициализирует их начальными данными.
		/// </summary>
		public static void InitializeDatabases(this IServiceScope serviceScope)
		{
			var dbCreationBindings = AssembliesManager.GetInstances<IDbCreationModuleBinding>();
			dbCreationBindings.ForEach(x =>
			{
				x.Create(serviceScope);
			});
		}

		/// <summary>
		/// Инициализирует базы данных начальными данными.
		/// </summary>
		public static void SeedInitialData(this IServiceScope serviceScope)
		{
			var initialDataBindings = AssembliesManager.GetInstances<IInitialDataModuleBinding>().OrderBy(x => x.Order);
			Task.Run(async () =>
			{
				foreach (var binding in initialDataBindings)
				{
					await binding.EnsureInitialData(serviceScope);
				}
			}).Wait();
		}
	}
}