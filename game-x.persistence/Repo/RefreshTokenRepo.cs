using EFCore.BulkExtensions;
using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Auth.Dtos;
using Polly;

namespace game_x.persistence.Repo;

public sealed class RefreshTokenRepo(GameXContext context) : IRefreshTokenRepo, IRepository
{
    public async Task<RefreshToken[]> GetActiveTokensAsync(CancellationToken ct = default)
    {
        return await context.RefreshTokens
            .AsNoTracking()
            .Where(rt => rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
            .ToArrayAsync(ct);
    }

    public async Task<RefreshToken?> FindActiveByTokenHashAsync(string tokenHash, CancellationToken ct = default)
    {
        return await context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt =>
                rt.TokenHash == tokenHash
                && rt.RevokedAt == null
                && rt.ExpiresAt > DateTime.UtcNow, ct);
    }

    public async Task<RefreshToken?> FindByIdAsync(Guid publicId, CancellationToken ct = default)
    {
        return await context.RefreshTokens
            .AsNoTracking()
            .FirstOrDefaultAsync(rt => rt.PublicId == publicId, ct);
    }

    public async Task AddRangeAsync(IEnumerable<RefreshToken> tokens, CancellationToken ct = default)
    {
        await context.RefreshTokens.AddRangeAsync(tokens, ct);
    }

    public async Task BulkRevokeAsync(IEnumerable<Guid> publicIds, DateTime revokedAtUtc, CancellationToken ct = default)
    {
        var targetItems = await context.RefreshTokens
            .Where(rt => publicIds.Contains(rt.PublicId) && rt.RevokedAt == null)
            .ToListAsync(ct);

        targetItems.ForEach(rt => rt.Revoke());
    }

    public async Task SyncRefreshTokensAsync(IEnumerable<RefreshTokenDto> tokens, CancellationToken ct = default)
    {
        // Get tokens that are needed to be updated
        var updateIds = tokens.Select(rt => rt.PublicId);
        var targetTokens = await context.RefreshTokens
            .AsNoTracking()
            .Where(rt => updateIds.Contains(rt.PublicId))
            .ToArrayAsync(ct);
        if (targetTokens.Length == 0) return;

        // Update tokens with new values
        foreach (var token in targetTokens)
        {
            var updateItem = tokens.FirstOrDefault(rt => rt.PublicId == token.PublicId);
            if (updateItem is null) continue;

            token.UpdateToSync(updateItem.ReplacedByToken, updateItem.RevokedAt);
        }

        // Perform bulk update
        context.IsDisableTimestamps = true;
        await context.BulkUpdateAsync(targetTokens, new BulkConfig
        {
            UpdateByProperties = [nameof(RefreshToken.PublicId)],
            PropertiesToInclude = [
                nameof(RefreshToken.RevokedAt),
                nameof(RefreshToken.ReplacedByToken)
            ]
        }, cancellationToken: ct);
        context.IsDisableTimestamps = false;
    }
}
