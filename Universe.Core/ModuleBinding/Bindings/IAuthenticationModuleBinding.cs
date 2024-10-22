using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Universe.Core.ModuleBinding.Bindings;

public interface IAuthenticationModuleBinding
{
	void SignUp(AuthenticationBuilder authBuilder, IServiceCollection services, IConfiguration configuration);
}
