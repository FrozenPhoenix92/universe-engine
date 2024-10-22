using Universe.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace Universe.Core.Audit;

public class AuditableDbContextCore : DbContextCore
{
    protected readonly IAuditService? _auditWrapper;


    public AuditableDbContextCore(DbContextOptions options, IDictionary<string, object>? features = null) : base(options) =>
        _auditWrapper = (IAuditService?) features?.Values.SingleOrDefault(x => x is IAuditService);


    public override int SaveChanges()
    {
        if (_auditWrapper is null) return base.SaveChanges();

        _auditWrapper.OnBeforeSaveChanges(ChangeTracker.Entries().ToArray());
        var result = base.SaveChanges();
        _auditWrapper.OnAfterSaveChanges();
        return result;
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (_auditWrapper is null) return await base.SaveChangesAsync(cancellationToken);

        _auditWrapper.OnBeforeSaveChanges(ChangeTracker.Entries().ToArray());
        var result = await base.SaveChangesAsync(cancellationToken);
        await _auditWrapper.OnAfterSaveChangesAsync(cancellationToken);
        return result;
    }
}
