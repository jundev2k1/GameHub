using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Jobs;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.infrastructure.BackgroundJobs.Jobs;

public sealed class ExpiredTransactionJob(
    IUnitOfWork unitOfWork,
    ITransactionRepo transactionRepo,
    IOptions<RecurringJobSettings> jobOptions,
    IOptions<UxmSettings> uxmSettings,
    IAppLogger<ExpiredTransactionJob> logger) : IRecurringJob
{
    public string JobId => "expired-transaction";
    public string CronExpression => jobOptions.Value.ExpiredTransactionJob;
    public bool IsInit => true;

    public async Task ExecuteAsync(CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                logger.LogInformation("Job is running...");
                int count = await transactionRepo.ExpiredTransactionAsync(uxmSettings.Value.TransactionExpireTimeSeconds, ct);
                logger.LogInformation("Expired {Count} transactions", count);
                logger.LogInformation("Job has stopped...");
            }
            catch (Exception ex)
            {
                logger.LogError($"Response failed: Message={ex.Message}");
            }
        }, ct);
    }
}