using game_x.domain.Entities.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface IUserEventRepo
{
    Task<UserEvent?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task AddAsync(UserEvent entity, CancellationToken ct = default);
    
    Task UpdateAsync(Guid id, Action<UserEvent> updateAction, CancellationToken ct = default);
}