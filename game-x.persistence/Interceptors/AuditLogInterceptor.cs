using game_x.domain.Enum;
using game_x.persistence.Extensions;
using game_x.share.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Security.Claims;

namespace game_x.persistence.Interceptors;

public sealed class AuditLogInterceptor(IHttpContextAccessor httpContextAccessor) : ISaveChangesInterceptor
{
    private readonly List<EntityEntry> _postSaveAddedEntries = [];

    public async Task OnBeforeSaveAsync(GameXContext context, CancellationToken ct = default)
    {
        var preSaveEntries = context.ChangeTracker
            .Entries<IAuditable>()
            .Where(x => (x.State is EntityState.Modified or EntityState.Deleted) && x.HasMeaningfulChanges())
            .ToList();
        foreach (var entry in preSaveEntries)
        {
            var auditLog = BuildAuditLog(entry);
            await context.AuditLogs.AddAsync(auditLog, ct);
        }

        _postSaveAddedEntries.Clear();
        _postSaveAddedEntries.AddRange(
            context.ChangeTracker
                .Entries<IAuditable>()
                .Where(e => e.State == EntityState.Added && e.HasMeaningfulChanges()));
    }

    public async Task OnAfterSaveAsync(GameXContext context, CancellationToken ct = default)
    {
        if (_postSaveAddedEntries.Count == 0)
            return;

        foreach (var entry in _postSaveAddedEntries)
        {
            var auditLog = BuildAuditLog(entry, true);
            await context.AuditLogs.AddAsync(auditLog, ct);
        }

        _postSaveAddedEntries.Clear();
        await context.SaveChangesAsync(ct);
    }

    private AuditLog BuildAuditLog(EntityEntry entry, bool isAdded = false)
    {
        var entityName = entry.Entity.GetType().Name;
        var entityId = entry.GetPrimaryKeyAsString() ?? "N/A";
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var action = !isAdded ? entry.ToAuditAction() : AuditAction.Created;
        var source = AuditSourceContext.Current;

        return AuditLog.Create(
            entityName: EntityName.Of(entityName),
            entityId: entityId,
            action: action,
            changedByUserId: userId,
            source: source,
            changes: entry.SerializeChanges(),
            snapshotBefore: entry.SerializeOriginal(),
            snapshotAfter: entry.SerializeCurrent());
    }
}
