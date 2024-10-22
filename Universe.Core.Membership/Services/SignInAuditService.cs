using Universe.Core.Audit;
using Universe.Core.Infrastructure;
using Universe.Core.Membership.Model;

using Microsoft.EntityFrameworkCore;

namespace Universe.Core.Membership.Services;

public class SignInAuditService : DataService<SignInAudit, int, IAuditDataContext>, ISignInAuditService
{
	public SignInAuditService(IAuditDataContext context) : base(context)
	{
	}


	public async Task<int> GetLastSignInAttemptsCountForIp(string ip, DateTime? withInDate, CancellationToken ct = default) =>
		await Context.Set<SignInAudit>().CountAsync(c => c.Ip == ip && c.DateTime > withInDate, ct);

	public async Task<SignInAudit?> GetLastSuccessfulSignInAuditForUser(string userEmail, CancellationToken ct = default) =>
		await Context.Set<SignInAudit>()
			.AsNoTracking()
			.Where(x => x.IdentityIdentifier == userEmail && x.Result == LoginAuditResult.Success)
			.OrderByDescending(x => x.DateTime)
			.FirstOrDefaultAsync(ct);
}
