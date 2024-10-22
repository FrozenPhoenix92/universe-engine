using Universe.Core.Membership.Model;
using Universe.Core.Membership.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Universe.Core.Membership.Authorization;

public class ExtendedUserManager : UserManager<User>
{
	private readonly ISecurityService _securityService;


	public ExtendedUserManager(
		IUserStore<User> store,
		IOptions<IdentityOptions> optionsAccessor,
		IPasswordHasher<User> passwordHasher,
		IEnumerable<IUserValidator<User>> userValidators,
		IEnumerable<IPasswordValidator<User>> passwordValidators,
		ILookupNormalizer keyNormalizer,
		IdentityErrorDescriber errors,
		IServiceProvider services,
		ILogger<UserManager<User>> logger,
		ISecurityService securityService)
		: base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
		=> _securityService = securityService;


	public override async Task<IdentityResult> CreateAsync(User user)
	{
		var result = await base.CreateAsync(user);

		if (!string.IsNullOrEmpty(user.PasswordHash))
		{
			await _securityService.SavePasswordToHistory(user);
		}

		return result;
	}
}
