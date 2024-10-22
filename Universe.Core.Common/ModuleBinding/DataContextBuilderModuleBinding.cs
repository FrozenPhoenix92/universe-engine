using Universe.Core.ModuleBinding.Bindings;

using Microsoft.EntityFrameworkCore;
using Universe.Core.Common.Model;

namespace Universe.Core.Common.ModuleBinding;

public class DataContextBuilderModuleBinding : IDataContextBuilderModuleBinding
{
	public string ModuleBindingId => CoreStaticData.MainDataContextBindingId;

	public void ConfigureModels(ModelBuilder builder)
	{
		builder.Entity<AppSettingsSet>();
		builder.Entity<AppSettingsSetRole>();
		builder.Entity<AppSettingsSetPermission>();
		builder.Entity<IpAddressConstraint>();
	}
}
