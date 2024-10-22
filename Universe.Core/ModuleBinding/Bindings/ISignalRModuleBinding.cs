using Microsoft.AspNetCore.Routing;

namespace Universe.Core.ModuleBinding.Bindings;

public interface ISignalRModuleBinding
{
	void MapHubs(IEndpointRouteBuilder routes);
}
