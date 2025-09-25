namespace game_x.application.Contract.Jobs;

public interface IRecurringJob
{
    string JobId { get; }
    string? CronExpression { get; }
    bool IsInit { get; }

    Task ExecuteAsync(CancellationToken cancellationToken);
}
