using game_x.application.Contract.Jobs;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.infrastructure.BackgroundJobs.Jobs;

public sealed class GameSessionTimeoutCheckerJob(
    IUnitOfWork unitOfWork,
    IUserGameSessionRepo userGameSessionRepo,
    IOptions<RecurringJobSettings> jobOptions) : IRecurringJob
{
    public string JobId => "game-session-timeout-checker";
    public string CronExpression => jobOptions.Value.GameSessionTimeoutCheckerJob;
    public bool IsInit => false;

    /// <summary>Maximum number of records allowed to be processed per transaction</summary>
    private const int LimitRangeCount = 1000;

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await userGameSessionRepo.BulkUpdateExpiredGameSessionsAsync(LimitRangeCount, ct);
        }, ct);
    }
}
