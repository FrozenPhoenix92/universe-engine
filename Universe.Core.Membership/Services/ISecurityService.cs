using Universe.Core.Membership.Model;

namespace Universe.Core.Membership.Services;

public interface ISecurityService
{
	Task<string> DecryptString(string value, CancellationToken ct = default);

	Task<string> EncryptString(string value, CancellationToken ct = default);

	Task SavePasswordToHistory(User user, bool saveChanges = true, CancellationToken cancellationToken = default);
}
