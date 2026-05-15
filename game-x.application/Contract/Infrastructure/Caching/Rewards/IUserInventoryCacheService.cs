using game_x.application.Features.Rewards.Dtos;

namespace game_x.application.Contract.Infrastructure.Caching.Rewards;

public interface IUserInventoryCacheService
{
    Task RefreshCache(string userId, CancellationToken ct = default);

    Task<UserInventoryDto[]?> GetAll(string userId, CancellationToken ct = default);
}