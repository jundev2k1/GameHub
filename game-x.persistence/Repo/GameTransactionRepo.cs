using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.domain.Constants;
using Microsoft.EntityFrameworkCore;

namespace game_x.persistence.Repo;

public sealed class GameTransactionRepo(GameXContext context) : IGameTransactionRepo
{
    public async Task<GameTransaction> AddAsync(GameTransaction entity, CancellationToken ct = default)
    {
        await context.GameTransactions.AddAsync(entity, ct);
        return entity;
    }
    
    public async Task PatchUpdateAsync(Guid publicId, Action<GameTransaction> updateAction, CancellationToken ct = default)
    {
        var transaction = await context.GameTransactions
                              .FirstOrDefaultAsync(c => c.PublicId == publicId, ct)
                               ?? throw new NotFoundException(MessageCode.Transaction.GameTransactionNotFound);

        updateAction.Invoke(transaction);
        await context.SaveChangesAsync(ct);
    }

    public async Task PutUpdateAsync(GameTransaction transaction, CancellationToken ct = default)
    {
        context.Entry(transaction).State = EntityState.Modified;
        await context.SaveChangesAsync(ct);
    }
}
