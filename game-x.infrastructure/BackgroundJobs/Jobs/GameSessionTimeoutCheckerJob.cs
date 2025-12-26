using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Jobs;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.infrastructure.BackgroundJobs.Jobs;

public sealed class GameSessionTimeoutCheckerJob(
    IUserGameSessionRepo userGameSessionRepo,
    IOptions<RecurringJobSettings> jobOptions,
    IAppLogger<GameSessionTimeoutCheckerJob> logger) : IRecurringJob
{
    public string JobId => "game-session-timeout-checker";
    public string CronExpression => jobOptions.Value.GameSessionTimeoutCheckerJob;
    public bool IsInit => true;

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        logger.LogInformation("Job is running...");
        await userGameSessionRepo.BulkUpdateExpiredGameSessionsAsync(ct);
        logger.LogInformation("Job has stopped...");
    }
}
