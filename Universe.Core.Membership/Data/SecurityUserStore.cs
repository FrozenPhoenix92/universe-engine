using Universe.Core.Membership.Model;
using Universe.Core.Membership.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Universe.Core.Membership.Data;

public class SecurityUserStore<TContext> : UserStore<User, Role, TContext, Guid, IdentityUserClaim<Guid>, UserRole,
	IdentityUserLogin<Guid>, IdentityUserToken<Guid>, IdentityRoleClaim<Guid>>
	where TContext : DbContext
{
	private readonly ISecurityService _securityService;

	public SecurityUserStore(TContext context, ISecurityService securityService, IdentityErrorDescriber? describer = null)
		: base(context, describer) => _securityService = securityService;

	public async override Task SetPasswordHashAsync(User user, string? passwordHash, CancellationToken cancellationToken = default)
	{
		await base.SetPasswordHashAsync(user, passwordHash, cancellationToken);

		if (user.Id != default)
		{
			await _securityService.SavePasswordToHistory(user, false);
		}
	}
}
