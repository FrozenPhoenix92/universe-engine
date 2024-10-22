using Universe.Core.AppConfiguration.Exceptions;
using Universe.Core.Audit;
using Universe.Core.Infrastructure;
using Universe.Core.Membership.Authorization;
using Universe.Core.Membership.Configuration;
using Universe.Core.Membership.Model;
using Universe.Core.ModuleBinding.Bindings;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Universe.Core.Membership.ModuleBindings;

public class DependenciesModuleBinding : IDependenciesModuleBinding
{
	public void AddDependencies(IServiceCollection services, IConfiguration configuration)
	{
		var identitySettingsSection = configuration.GetSection(nameof(IdentitySettings));
		var identitySettings = identitySettingsSection?.Get<IdentitySettings>();

		if (identitySettingsSection is null || identitySettings is null)
		{
			throw new MissedConfigurationSectionException(nameof(IdentitySettings));
		}

		services.Configure<IdentitySettings>(identitySettingsSection);


		var securitySettingsSection = configuration.GetSection(nameof(SecuritySettings));
		var securitySettings = securitySettingsSection?.Get<SecuritySettings>();

		if (securitySettingsSection is null || securitySettings is null)
		{
			throw new MissedConfigurationSectionException(nameof(SecuritySettings));
		}

		if (string.IsNullOrEmpty(securitySettings.SecureDataEncryptionKey))
		{
			throw new MissedConfigurationSectionException(nameof(SecuritySettings.SecureDataEncryptionKey));
		}

		services.Configure<SecuritySettings>(securitySettingsSection);


		services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
		services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
		services.AddScoped<PermissionManager>();
		services.AddScoped<IDataService<User, Guid>, DataService<User, Guid>>();
		services.AddScoped<IDataService<Role, Guid>, DataService<Role, Guid>>();
		services.AddScoped<IDataService<Permission, Guid>, DataService<Permission, Guid>>();
		services.AddScoped<IDataService<SignInAudit, int, IAuditDataContext>, DataService<SignInAudit, int, IAuditDataContext>>();
		services.AddScoped<IClaimsTransformation, TwoFactorRequiredRoleClaimsTransformation>();
	}
}
