using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using Microsoft.EntityFrameworkCore.Storage;

namespace game_x.persistence;

public sealed class EfUnitOfWork(GameXContext dbContext) : IUnitOfWork
{
    private IDbContextTransaction? _tx;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _tx ??= await dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency conflict on SaveChanges", ex);
        }
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_tx is null) return;

        await SaveChangesAsync(ct);
        await _tx.CommitAsync(ct);
        await _tx.DisposeAsync();
        _tx = null;
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_tx is null) return;

        await _tx.RollbackAsync(ct);
        await _tx.DisposeAsync();
        _tx = null;
        dbContext.ChangeTracker.Clear();
    }

    public async Task WithTransactionAsync(Func<Task> action, CancellationToken ct = default)
    {
        await BeginTransactionAsync(ct);
        try
        {
            await action();
            await CommitAsync(ct);
        }
        catch
        {
            await RollbackAsync(ct);
            throw;
        }
    }
}
