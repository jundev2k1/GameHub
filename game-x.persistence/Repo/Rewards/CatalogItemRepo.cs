using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Exceptions;
using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Constants;
using game_x.domain.Entities.Rewards;
using Mapster;

namespace game_x.persistence.Repo.Rewards;

public sealed class CatalogItemRepo(
    GameXContext dbContext,
    IFileManagerCacheService storage) : ICatalogItemRepo, IRepository
{
    public async Task<CatalogItemDto[]> GetListAsync(CancellationToken ct = default)
    {
        var items = await dbContext.CatalogItems
            .AsNoTracking()
            .Include(x => x.Icon)
            .ToListAsync(ct);

        var tasks = items.Select(async item =>
        {
            var dto = item.Adapt<CatalogItemDto>();

            dto.IconUrl = item.Icon is null
                ? null
                : await storage.GetFileUrl(item.Icon, ct);

            return dto;
        });

        return await Task.WhenAll(tasks);
    }
    
    public async Task<bool> CheckExistedCodeAsync(string code, CancellationToken ct = default)
    {
        return await dbContext.CatalogItems
            .AnyAsync(x => x.Code == code, ct);
    }

    public Task<bool> ExistsByCatalogItemIdAsync(int catalogItemId, CancellationToken ct = default)
    {
        return dbContext.RewardDefinitions
            .AnyAsync(x => x.CatalogItemId == catalogItemId, ct);
    }
    
    public async Task<CatalogItem> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.CatalogItems
            .FirstOrDefaultAsync(x => x.PublicId == id, ct)
            ?? throw new NotFoundException((MessageCode.Reward.CatalogNotFound));
    }

    public async Task<CatalogItem> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return await dbContext.CatalogItems
            .FirstOrDefaultAsync(x => x.Code == code, ct)
            ?? throw new NotFoundException((MessageCode.Reward.CatalogNotFound));
    }
    
    public async Task AddAsync(CatalogItem entity, CancellationToken ct = default)
    {
        await dbContext.CatalogItems.AddAsync(entity, ct);
    }
    
    public async Task UpdateAsync(Guid id, Action<CatalogItem> updateAction, CancellationToken ct = default)
    {
        var entity = await dbContext.CatalogItems
                         .FirstOrDefaultAsync(c => c.PublicId == id, ct)
                     ?? throw new NotFoundException(MessageCode.Reward.CatalogNotFound);

        updateAction.Invoke(entity);
    }
    
    public async Task RemoveAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await dbContext.CatalogItems.FirstOrDefaultAsync(x => x.PublicId == id, ct);
        if (entity is null)
            throw new NotFoundException(MessageCode.Reward.CatalogNotFound);
        
        dbContext.CatalogItems.Remove(entity);
    }
}