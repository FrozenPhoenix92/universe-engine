using Universe.Core;
using Universe.Core.ModuleBinding.Bindings;
using Universe.Files.Model;

using Microsoft.EntityFrameworkCore;

namespace Universe.Web.ModuleBinding;

public class DataContextBuilderModuleBinding : IDataContextBuilderModuleBinding
{
	public string ModuleBindingId => CoreStaticData.MainDataContextBindingId;

	public void ConfigureModels(ModelBuilder builder)
	{
		builder.Entity<FileDetails>();
	}
}
