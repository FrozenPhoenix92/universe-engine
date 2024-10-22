using Universe.Core.Membership.Model;

using Microsoft.AspNetCore.Identity;

namespace Universe.Core.Membership.Authorization;

public class ApprovanceUserConfirmation : IUserConfirmation<User>
{
	public Task<bool> IsConfirmedAsync(UserManager<User> manager, User user) => Task.FromResult(user.Approved);
}
