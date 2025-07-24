using game_x.domain.Enum;
using game_x.share.Extensions;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace game_x.persistence.Extensions;

public static class EntityEntryExtensions
{
    private static readonly string[] _ignoreFields = [
        nameof(AppUser.ConcurrencyStamp),
        nameof(AppUser.SecurityStamp),
        nameof(AppUser.LockoutEnd),
        nameof(BaseEntity<object>.CreatedAt),
        nameof(BaseEntity<object>.UpdatedAt)];

    public static string? GetPrimaryKeyAsString(this EntityEntry entry)
    {
        var keyProps = entry.Properties
            .Where(p => p.Metadata.IsPrimaryKey())
            .OrderBy(p => p.Metadata.Name)
            .ToList();
        if (keyProps.Count == 0) return null;

        var result = keyProps
            .Select(p => $"{p.Metadata.Name}:{p.CurrentValue}")
            .JoinToString("|");
        return result;
    }

    public static AuditAction ToAuditAction(this EntityEntry entry)
    {
        return entry.State switch
        {
            EntityState.Added => AuditAction.Created,
            EntityState.Modified => AuditAction.Updated,
            EntityState.Deleted => AuditAction.Deleted,
            _ => throw new ArgumentOutOfRangeException(nameof(entry.State), "Unsupported entity state for audit action.")
        };
    }

    public static string? SerializeChanges(this EntityEntry entry)
    {
        if (entry.State != EntityState.Modified)
            return null;

        var changes = entry.Properties
            .Where(p => p.IsModified
                && !_ignoreFields.Contains(p.Metadata.Name)
                && !Equals(p.OriginalValue, p.CurrentValue))
            .Select(p => new
            {
                Field = p.Metadata.Name,
                Before = p.OriginalValue,
                After = p.CurrentValue
            });
        return changes.Any() ? JsonSerializer.Serialize(changes) : null;
    }

    public static string? SerializeOriginal(this EntityEntry entry)
    {
        if (entry.State == EntityState.Added) return null;

        var original = entry.Properties
            .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
        return JsonSerializer.Serialize(original);
    }

    public static string? SerializeCurrent(this EntityEntry entry)
    {
        if (entry.State == EntityState.Deleted) return null;

        var current = entry.Properties
            .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
        return JsonSerializer.Serialize(current);
    }

    public static bool HasMeaningfulChanges(this EntityEntry entry)
    {
        return entry.State switch
        {
            EntityState.Added => entry.Properties
                .Any(p => !_ignoreFields.Contains(p.Metadata.Name)),

            EntityState.Modified => entry.Properties
                .Any(p => p.IsModified
                    && !Equals(p.OriginalValue, p.CurrentValue)
                    && !_ignoreFields.Contains(p.Metadata.Name)),

            EntityState.Deleted => true,
            _ => false
        };
    }
}
