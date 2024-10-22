using Universe.Core.Audit;
using Universe.Core.Infrastructure;
using Universe.Core.Membership.Model;

namespace Universe.Core.Membership.Services;

public interface ISignInAuditService : IDataService<SignInAudit, int, IAuditDataContext>
{
    Task<int> GetLastSignInAttemptsCountForIp(string ip, DateTime? withInDate = null, CancellationToken ct = default);

	Task<SignInAudit?> GetLastSuccessfulSignInAuditForUser(string userEmail, CancellationToken ct = default);
}
