using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;

namespace game_x.persistence.Repo.Rewards;

public sealed class IdempotencyKeyRepo(GameXContext dbContext) : IIdempotencyKeyRepo, IRepository
{
    public async Task<IdempotencyKey?> GetForUpdateAsync(
        string key,
        string userId,
        IdempotencyActionType actionType,
        CancellationToken ct)
    {
        return await dbContext.IdempotencyKeys
            .Where(x => 
                x.UserId == userId && 
                x.Key == key &&
                x.ActionType == actionType)
            .FirstOrDefaultAsync(ct);
    }
    
    public async Task AddAsync(IdempotencyKey entity, CancellationToken ct)
    {
        await dbContext.IdempotencyKeys.AddAsync(entity, ct);
    }
    
    public async Task RemoveAsync(
        string key,
        string userId,
        IdempotencyActionType actionType,
        CancellationToken ct)
    {
        var entity = await dbContext.IdempotencyKeys.FirstOrDefaultAsync(
            x => x.Key == key &&
                 x.UserId == userId &&
                 x.ActionType == actionType,
            ct);

        if (entity == null)
            return;

        dbContext.IdempotencyKeys.Remove(entity);
    }
}