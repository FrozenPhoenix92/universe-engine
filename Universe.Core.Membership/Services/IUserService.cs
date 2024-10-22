using Universe.Core.Infrastructure;
using Universe.Core.Membership.Dto;
using Universe.Core.Membership.Model;

namespace Universe.Core.Membership.Services;

public interface IUserService :
	IValidateDataOperation<UserDto, User, Guid>,
	ICreateDataOperation<User, Guid>,
	IUpdateDataOperation<User, Guid>,
	IChangeQueryDataOperation<User>
{
	Task<User> Create(User user, string? password = null, CancellationToken ct = default);
}
