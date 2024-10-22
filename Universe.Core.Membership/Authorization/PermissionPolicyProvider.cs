using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Universe.Core.Membership.Authorization;

internal class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
	public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) =>
		DefaultPolicyProvider = new DefaultAuthorizationPolicyProvider(options);


	public DefaultAuthorizationPolicyProvider DefaultPolicyProvider { get; }

	public async Task<AuthorizationPolicy> GetDefaultPolicyAsync() => await DefaultPolicyProvider.GetDefaultPolicyAsync();

	public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => await DefaultPolicyProvider.GetFallbackPolicyAsync();


	public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
	{
		if (policyName.StartsWith(CorePolicies.PermissionPolicyName, StringComparison.OrdinalIgnoreCase))
		{
			var policy = new AuthorizationPolicyBuilder();
			policy.AddRequirements(new PermissionRequirement(policyName));
			return policy.Build();
		}

		return await DefaultPolicyProvider.GetPolicyAsync(policyName);
	}
}
