using Universe.Core.Data;
using Universe.Core.ModuleBinding.Bindings;
using Universe.Core.Utils;

using Microsoft.EntityFrameworkCore;

namespace Universe.Core.ModuleBinding;

/// <summary>
/// Базовый класс контекста данных, поддерживающий настройку моделей другими модулями.
/// </summary>
public abstract class ConfigurableFromModulesDbContextCore : DbContextCore
{
	public ConfigurableFromModulesDbContextCore(DbContextOptions options) : base(options) { }


	/// <summary>
	/// Возвращает уникальное значение контекста, с помощью которого осуществляется поиск и применение внешних настроек моделей из других модулей.
	/// </summary>
	public abstract string ModuleBindingId { get; }


	public static void ApplyModelsConfiguration(ModelBuilder modelBuilder, string contextModuleBindingId)
	{
		VariablesChecker.CheckIsNotNullOrEmpty(contextModuleBindingId, nameof(contextModuleBindingId));

		var modelsBinders = AssembliesManager.GetInstances<IDataContextBuilderModuleBinding>()
			.Where(x => !string.IsNullOrEmpty(x.ModuleBindingId) && x.ModuleBindingId == contextModuleBindingId);

		foreach (var modelsBinder in modelsBinders)
		{
			modelsBinder.ConfigureModels(modelBuilder);
		}
	}


	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		ApplyModelsConfiguration(modelBuilder, ModuleBindingId);
	}
}
