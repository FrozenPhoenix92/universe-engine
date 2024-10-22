using Universe.Core.Membership.Model;

using Microsoft.AspNetCore.Identity;

namespace Universe.Core.Membership.Authorization;

public class RepeatingPasswordValidator : IPasswordValidator<User>
{
	public async Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string? password)
	{
		if (!await manager.HasPasswordAsync(user))
			return IdentityResult.Success;

		return manager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash!, password) == PasswordVerificationResult.Failed
			? IdentityResult.Success
			: IdentityResult.Failed(new IdentityError 
			{
				Code = "EqualsToPrevious", 
				Description = "Новый пароль не может быть равен предыдущему." 
			});
	}
}
