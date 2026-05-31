using System.Linq.Expressions;
using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Exceptions;
using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Constants;
using game_x.domain.Entities.Rewards;
using Mapster;

namespace game_x.persistence.Repo.Rewards;

public sealed class RewardPoolItemRepo(
    GameXContext dbContext,
    IFileManagerCacheService storage) : IRewardPoolItemRepo, IRepository
{
    public async Task<RewardPoolItemDto[]> GetAllByAdminAsync(int poolId, CancellationToken ct = default)
    {
        var items = await dbContext.RewardPoolItems
            .AsNoTracking()
            .OrderByDescending(x => x.SortOrder)
            .Include(x => x.RewardDefinition)
                .ThenInclude(x => x!.CatalogItem)
                    .ThenInclude(x => x!.Icon)
            .Where(x => x.RewardPoolId == poolId)
            .ToArrayAsync(ct);
        
        return await Task.WhenAll(
            items.Select(async item =>
            {
                var dto = item.Adapt<RewardPoolItemDto>();
                dto.ItemIconUrl = item.RewardDefinition?.CatalogItem?.Icon is null
                    ? null
                    : await storage.GetFileUrl(item.RewardDefinition.CatalogItem.Icon, ct);

                return dto;
            }));
    }
    
    public async Task<RewardPoolItemDto[]> GetAllByUserAsync(int poolId, CancellationToken ct = default)
    {
        var items = await dbContext.RewardPoolItems
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.SortOrder)
            .Include(x => x.RewardDefinition)
                .ThenInclude(x => x!.CatalogItem)
                    .ThenInclude(x => x!.Icon)
            .Where(x => x.RewardPoolId == poolId)
            .ToArrayAsync(ct);
        
        return await Task.WhenAll(
            items.Select(async item =>
            {
                var dto = item.Adapt<RewardPoolItemDto>();
                dto.ItemIconUrl = item.RewardDefinition?.CatalogItem?.Icon is null
                    ? null
                    : await storage.GetFileUrl(item.RewardDefinition.CatalogItem.Icon, ct);

                return dto;
            }));
    }
    
    public async Task<RewardPoolItem> GetDetailByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.RewardPoolItems
            .AsNoTracking()
            .Include(x => x.RewardPool)
            .Include(x => x.RewardDefinition)
            .FirstOrDefaultAsync(x => x.PublicId == id, ct)
            ?? throw new BadRequestException(MessageCode.Reward.RewardPoolItemNotFound);
    }
    
    public async Task<IReadOnlyCollection<RewardPoolItem>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        return await dbContext.RewardPoolItems
            .AsNoTracking()
            .Include(x => x.RewardPool)
            .Where(x => ids.Contains(x.PublicId))
            .ToListAsync(ct);
    }
    
    public async Task<ICollection<RewardPoolItem>> GetByIdsForUpdateAsync(IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        return await dbContext.RewardPoolItems
            .Include(x => x.RewardPool)
            .Where(x => ids.Contains(x.PublicId))
            .ToListAsync(ct);
    }
    
    public Task<bool> ExistsByRewardIdAsync(int rewardId, CancellationToken ct = default)
    {
        return dbContext.RewardPoolItems
            .AnyAsync(x => x.RewardDefinitionId == rewardId, ct);
    }
    
    public async Task AddRangeAsync(IEnumerable<RewardPoolItem> items, CancellationToken ct = default)
    {
        await dbContext.RewardPoolItems.AddRangeAsync(items, ct);
    }
    
    public async Task BulkDeleteAsync(Expression<Func<RewardPoolItem, bool>> predicate, CancellationToken ct = default)
    {
        await dbContext.RewardPoolItems.Where(predicate).ExecuteDeleteAsync(ct);
    }
}