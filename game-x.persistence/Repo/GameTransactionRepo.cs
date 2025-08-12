using game_x.application.Contract.Persistence.Repo;
using Microsoft.EntityFrameworkCore;

namespace game_x.persistence.Repo;

public sealed class GameTransactionRepo(GameXContext context) : IGameTransactionRepo
{
    public async Task<GameTransaction> AddAsync(GameTransaction entity, CancellationToken ct = default)
    {
        await context.GameTransactions.AddAsync(entity, ct);
        return entity;
    }
}
