namespace game_x.application.Contract.Persistence.Repo;

public interface IUnitOfWork
{
    Task BeginTransactionAsync(CancellationToken ct = default);

    Task SaveChangesAsync(CancellationToken ct = default);

    Task CommitAsync(CancellationToken ct = default);

    Task RollbackAsync(CancellationToken ct = default);

    Task WithTransactionAsync(Func<Task> action, CancellationToken ct = default);

    void SetIsDisableTimeStamps(bool isDisable = false);
}
