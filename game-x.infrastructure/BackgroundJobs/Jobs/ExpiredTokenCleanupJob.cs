using game_x.application.Contract.Jobs;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace game_x.infrastructure.BackgroundJobs.Jobs;

public sealed class ExpiredTokenCleanupJob(
    IUnitOfWork unitOfWork,
    IRefreshTokenRepo refreshTokenRepo,
    GameXContext dbContext,
    IOptions<RecurringJobSettings> jobOptions) : IRecurringJob
{
    public string JobId => "expired-token-cleanup";
    public string CronExpression => jobOptions.Value.ExpiredTokenCleanupJob;
    public bool IsInit => false;

    /// <summary>Maximum number of records allowed to be processed per transaction</summary>
    private const int DayOverdueCount = 3;

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        var isContinue = true;
        var expiredDate = DateTime.UtcNow.AddDays(-DayOverdueCount);
        while (isContinue)
        {
            try
            {
                var refreshTokenIds = await dbContext.RefreshTokens
                    .Where(rt => rt.ExpiresAt < expiredDate)
                    .Select(rt => rt.PublicId)
                    .ToArrayAsync(ct);
                if (refreshTokenIds.Length == 0)
                {
                    isContinue = false;
                    return;
                }

                await unitOfWork.WithTransactionAsync(async () =>
                {
                    await refreshTokenRepo.BulkDeleteAsync(refreshTokenIds, ct);
                }, ct);
            }
            catch { }
        }
    }
}
