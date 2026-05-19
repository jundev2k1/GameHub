using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Exceptions;
using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Constants;
using game_x.domain.Entities.Rewards;
using Mapster;

namespace game_x.persistence.Repo.Rewards;

public sealed class RewardDefinitionRepo(
    GameXContext dbContext,
    IFileManagerCacheService storage) : IRewardDefinitionRepo, IRepository
{
    public async Task<RewardDefinitionDto[]> GetListAsync(CancellationToken ct = default)
    {
        var items = await dbContext.RewardDefinitions
            .AsNoTracking()
            .Include(x => x.CatalogItem)
                .ThenInclude(x => x!.Icon)
            .ToListAsync(ct);

        var tasks = items.Select(async item =>
        {
            var dto = item.Adapt<RewardDefinitionDto>();
            if(item.CatalogItem != null) 
            {
                dto.ItemIconUrl = item.CatalogItem?.Icon is null
                    ? null
                    : await storage.GetFileUrl(item.CatalogItem?.Icon, ct);
            }
            
            return dto;
        });

        return await Task.WhenAll(tasks);
    }
    
    public async Task<bool> CheckExistedCodeAsync(string code, CancellationToken ct = default)
    {
        return await dbContext.RewardDefinitions
            .AnyAsync(x => x.Code == code, ct);
    }
    
    public async Task<RewardDefinition> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.RewardDefinitions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PublicId == id, ct)
            ?? throw new BadRequestException(MessageCode.Reward.RewardDefinitionNotFound);
    }
    
    public async Task AddAsync(RewardDefinition entity, CancellationToken ct = default)
    {
        await dbContext.RewardDefinitions.AddAsync(entity, ct);
    }
    
    public async Task UpdateAsync(Guid id, Action<RewardDefinition> updateAction, CancellationToken ct = default)
    {
        var entity = await dbContext.RewardDefinitions
                     .FirstOrDefaultAsync(c => c.PublicId == id, ct)
                 ?? throw new NotFoundException(MessageCode.Reward.RewardDefinitionNotFound);

        updateAction.Invoke(entity);
    }
    
    public async Task RemoveAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await dbContext.RewardDefinitions
            .FirstOrDefaultAsync(x => x.PublicId == id, ct);

        if (entity is null)
            throw new NotFoundException(MessageCode.Reward.RewardDefinitionNotFound);

        dbContext.RewardDefinitions.Remove(entity);
    }
}