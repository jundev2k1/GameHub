using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface IIdempotencyKeyRepo
{
    Task<IdempotencyKey?> GetForUpdateAsync(
        string key,
        string userId,
        IdempotencyActionType actionType,
        CancellationToken ct);
    
    Task AddAsync(IdempotencyKey entity, CancellationToken ct);

    Task RemoveAsync(
        string key,
        string userId,
        IdempotencyActionType actionType,
        CancellationToken ct);
}