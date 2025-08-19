using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Jobs;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Auth.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.infrastructure.BackgroundJobs.Jobs;

public sealed class SyncRefreshTokenJob(
    IRefreshTokenManagerCacheService refreshTokenManager,
    IUnitOfWork unitOfWork,
    IRefreshTokenRepo refreshTokenRepo,
    IOptions<RecurringJobSettings> jobOptions) : IRecurringJob
{
    public string JobId => "sync-refresh-token";
    public string CronExpression => jobOptions.Value.SyncRefreshTokenJob;
    public bool IsInit => false;

    /// <summary>Maximum number of records allowed to be processed per transaction</summary>
    private const int LimitRangeCount = 1;

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        var unSyncTokens = refreshTokenManager.GetAllTokens()
            .Where(rt => !rt.IsSynced)
            .ToArray();

        // If there are no unsynced tokens, insert new tokens
        var newItems = unSyncTokens
            .Where(rt => rt.State == SyncState.NotSynced)
            .ToArray();
        await AddNewTokens(newItems, ct);

        // If there are tokens that are updated, update them
        var updateItems = unSyncTokens
            .Where(rt => rt.State == SyncState.Updated)
            .ToArray();
        await UpdateTokens(updateItems, ct);

        // Remove tokens that are marked as revoked or expired
        refreshTokenManager.RemoveExpiredTokens();
    }

    private async Task AddNewTokens(RefreshTokenDto[] newItems, CancellationToken ct = default)
    {
        var batches = newItems.Chunk(LimitRangeCount);
        foreach (var batch in batches)
        {
            var newTokens = batch
                .Select(CreateEntity)
                .ToArray();

            unitOfWork.SetIsDisableTimeStamps(true);
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await refreshTokenRepo.AddRangeAsync(newTokens, ct);
            }, ct);

            // Mark tokens as synced after saving
            var updateIds = newTokens
                .Select(t => t.PublicId)
                .ToArray();
            refreshTokenManager.UpdateAfterSync(updateIds);
        }
    }

    private async Task UpdateTokens(RefreshTokenDto[] updateItems, CancellationToken ct = default)
    {
        var batches = updateItems.Chunk(LimitRangeCount);
        foreach (var batch in batches)
        {
            unitOfWork.SetIsDisableTimeStamps(true);
            await unitOfWork.WithTransactionAsync(async () =>
            {
                await refreshTokenRepo.SyncRefreshTokensAsync(batch, ct);
            }, ct);

            // Mark tokens as synced after saving
            var updateIds = batch
                .Select(t => t.PublicId)
                .ToArray();
            refreshTokenManager.UpdateAfterSync(updateIds);
        }
    }

    private static RefreshToken CreateEntity(RefreshTokenDto dto)
    {
        return RefreshToken.Create(
            userId: dto.UserId,
            tokenHash: dto.TokenHash,
            jwtId: dto.JwtId,
            expiresAt: dto.ExpiresAt,
            ipAddress: dto.IpAddress,
            userAgent: dto.UserAgent,
            deviceInfo: dto.DeviceInfo,
            location: dto.Location,
            publicId: dto.PublicId,
            replacedByToken: dto.ReplacedByToken,
            revokedAt: dto.RevokedAt,
            createdAt: dto.CreatedAt);
    }
}
