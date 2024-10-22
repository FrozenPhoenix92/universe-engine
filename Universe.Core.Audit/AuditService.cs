using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Universe.Core.Audit;

public interface IAuditService
{
    void OnBeforeSaveChanges(IEnumerable<EntityEntry> entries);

    void OnAfterSaveChanges();

    Task OnAfterSaveChangesAsync(CancellationToken cancellationToken = default);
}

public class AuditService : IAuditService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuditDataContext _auditDataContext;
    private AuditChangeEntry[] _auditEntries = Array.Empty<AuditChangeEntry>();


    public AuditService(IAuditDataContext auditDataContext, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _auditDataContext = auditDataContext;
    }


    public void OnBeforeSaveChanges(IEnumerable<EntityEntry> entries) =>
        _auditEntries = GetAuditEntries(entries).ToArray();

    public void OnAfterSaveChanges()
    {
        if (!_auditEntries.Any()) return;

        foreach (var auditEntry in _auditEntries)
        {
            foreach (var prop in auditEntry.KeyProperties)
            {
                if (prop.Metadata.IsPrimaryKey() && prop.CurrentValue is int)
                {
                    auditEntry.EntityId = (int)prop.CurrentValue;
                }
                else
                {
                    auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                }
            }

            _auditDataContext.ChangeLogs.Add(auditEntry.ToAudit());
        }

        _auditDataContext.SaveChanges();
    }

    public async Task OnAfterSaveChangesAsync(CancellationToken cancellationToken = default)
    {
        if (!_auditEntries.Any()) return;

        foreach (var auditEntry in _auditEntries)
        {
            foreach (var prop in auditEntry.KeyProperties)
            {
                if (prop.Metadata.IsPrimaryKey() && prop.CurrentValue is int)
                {
                    auditEntry.EntityId = (int)prop.CurrentValue;
                }
                else
                {
                    auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                }
            }

            await _auditDataContext.ChangeLogs.AddAsync(auditEntry.ToAudit(), cancellationToken);
        }

        await _auditDataContext.SaveChangesAsync(cancellationToken);
    }


    private static void SetAuditEntryByEntryProperties(EntityEntry entry, AuditChangeEntry auditEntry)
    {
        foreach (var property in entry.Properties)
        {
            var propertyName = property.Metadata.Name;

            if (property.Metadata.IsPrimaryKey() && property.CurrentValue is int)
            {
                auditEntry.EntityId = (int)property.CurrentValue;
            }
            else
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        break;

                    case EntityState.Deleted:
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;

                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            auditEntry.OldValues[propertyName] = property.OriginalValue;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;

                    default:
                        // Other states are not handled
                        break;
                }
            }

            if (property.Metadata.IsKey() || property.Metadata.IsForeignKey())
            {
                auditEntry.KeyProperties.Add(property);
            }
        }
    }

    private IEnumerable<AuditChangeEntry> GetAuditEntries(IEnumerable<EntityEntry> entries)
    {
        foreach (var entry in entries ?? Enumerable.Empty<EntityEntry>())
        {
            if (!entry.Entity.GetType().IsDefined(typeof(NonAuditableEntityAttribute), false) &&
                entry.State != EntityState.Detached &&
                entry.State != EntityState.Unchanged)
            {
                yield return GetAuditEntry(entry);
            }
        }
    }

    private AuditChangeEntry GetAuditEntry(EntityEntry entry)
    {
        var auditEntry = new AuditChangeEntry(entry)
        {
            TableName = entry.Metadata.GetTableName(),
            EntityName = entry.Entity.GetType().Name
        };

        var user = _httpContextAccessor?.HttpContext?.User;
        if (user is not null)
        {
            auditEntry.UserName = user.Identity.Name;
        }

        SetAuditEntryByEntryProperties(entry, auditEntry);

        return auditEntry;
    }
}
