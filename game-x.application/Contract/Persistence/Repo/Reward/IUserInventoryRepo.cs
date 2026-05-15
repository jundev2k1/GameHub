using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Entities.Rewards;

namespace game_x.application.Contract.Persistence.Repo.Reward;

public interface IUserInventoryRepo
{
    Task<UserInventoryDto[]> GetListAsync(string userId, CancellationToken ct = default);
    
    Task<UserInventory?> GetDetailAsync(string userId, int catalogItemId, CancellationToken ct = default);
    
    Task<UserInventory?> GetDetailAsync(string userId, Guid catalogItemId, CancellationToken ct = default);
    
    Task AddAsync(UserInventory entity, CancellationToken ct = default);
}