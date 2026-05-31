using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Entities.Rewards;
using Mapster;

namespace game_x.persistence.Repo.Rewards;

public sealed class UserInventoryRepo(
    GameXContext dbContext,
    IFileManagerCacheService storage) : IUserInventoryRepo, IRepository
{
    public async Task<UserInventoryDto[]> GetListAsync(string userId, CancellationToken ct = default)
    {
        var items = await dbContext.UserInventories
            .AsNoTracking()
            .Include(x => x.Item)
                .ThenInclude(i => i!.Icon)
            .OrderByDescending(x => x.Item!.SortOrder)
            .Where(x => x.UserId == userId)
            .ToArrayAsync(ct);
        
        var tasks = items.Select(async item =>
        {
            var dto = item.Adapt<UserInventoryDto>();

            dto.IconUrl = item.Item?.Icon is null
                ? null
                : await storage.GetFileUrl(item.Item.Icon, ct);

            return dto;
        });

        return await Task.WhenAll(tasks);
    }
    
    public async Task<UserInventory?> GetDetailAsync(string userId, int catalogItemId, CancellationToken ct = default)
    {
        return await dbContext.UserInventories
            .FirstOrDefaultAsync(x => x.UserId == userId && x.CatalogItemId == catalogItemId, ct);
    }
    
    public async Task<UserInventory?> GetDetailAsync(string userId, Guid catalogItemId, CancellationToken ct = default)
    {
        return await dbContext.UserInventories
            .Include(x => x.Item)
            .FirstOrDefaultAsync(x => 
                x.UserId == userId &&
                x.Item != null &&
                x.Item.PublicId == catalogItemId, ct);
    }
    
    public async Task AddAsync(UserInventory entity, CancellationToken ct = default)
    {
        await dbContext.UserInventories.AddAsync(entity, ct);
    }
}