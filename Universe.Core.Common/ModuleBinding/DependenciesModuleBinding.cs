using Universe.Core.Infrastructure;
using Universe.Core.ModuleBinding.Bindings;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Universe.Core.Common.ModuleBinding;

public class DependenciesModuleBinding : IDependenciesModuleBinding
{
	public void AddDependencies(IServiceCollection services, IConfiguration configuration)
	{
		services.AddScoped(typeof(IDataService<>), typeof(DataService<>));
	}
}
