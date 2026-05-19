using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Exceptions;
using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Constants;
using game_x.domain.Entities.Rewards;
using Mapster;

namespace game_x.persistence.Repo.Rewards;

public sealed class MissionRepo(GameXContext dbContext) : IMissionRepo, IRepository
{
    public async Task<ListedMissionDto[]> GetListAsync(CancellationToken ct = default)
    {
        return await dbContext.Missions
            .AsNoTracking()
            .ProjectToType<ListedMissionDto>()
            .ToArrayAsync(ct);
    }
    
    public async Task<MissionDto> GetDetailAsync(Guid id, CancellationToken ct = default)
    {
        var mission = await dbContext.Missions
            .AsNoTracking()
            .Where(m => m.PublicId == id)
            .Select(m => new MissionDto
            {
                Id = m.PublicId,
                Code = m.Code,
                Type = m.Type,
                Title = m.Title,
                Description = m.Description,
                ResetType = m.ResetType,
                IsActive = m.IsActive,
                ConfigData = m.ConfigData,
                StartAt = m.StartAt,
                EndAt = m.EndAt,
                MissionRewards = m.MissionRewards.Select(mr => new MissionRewardDto
                {
                    Id = mr.PublicId,
                    Sequence = mr.Sequence,
                    SortOrder = mr.SortOrder,
                    RequiredProgress = mr.RequiredProgress,
                    IsClaimable = mr.IsClaimable,
                    IsActive = mr.IsActive,
                    StartAt = mr.StartAt,
                    EndAt = mr.EndAt,
                    RewardDefinitionId = mr.RewardDefinition != null ? mr.RewardDefinition.PublicId : Guid.Empty,
                    Amount = mr.RewardDefinition != null ? mr.RewardDefinition.Amount : null,
                    Title = mr.RewardDefinition != null ? mr.RewardDefinition.Title : String.Empty,
                    Description = mr.RewardDefinition != null ? mr.RewardDefinition.Description : null,
                    ItemId = mr.RewardDefinition != null && mr.RewardDefinition.CatalogItem != null
                        ? mr.RewardDefinition.CatalogItem.PublicId : null,
                    ItemName = mr.RewardDefinition != null && mr.RewardDefinition.CatalogItem != null
                        ? mr.RewardDefinition.CatalogItem.Name : null,
                    RewardType = mr.RewardDefinition != null ? mr.RewardDefinition.Type : null,
                    ItemIconType = mr.RewardDefinition != null && mr.RewardDefinition.CatalogItem != null 
                        ? mr.RewardDefinition.CatalogItem.IconType : null,
                    ItemIcon = mr.RewardDefinition != null && mr.RewardDefinition.CatalogItem != null 
                        ? mr.RewardDefinition.CatalogItem.Icon : null,
                    
                }).ToArray()
            })
            .FirstOrDefaultAsync(ct);
            
            if(mission == null)
            throw new NotFoundException(MessageCode.Reward.MissionNotFound);
    
            return mission;
    }
    
    public async Task<Mission> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Missions
                   .AsNoTracking()
                   .Where(x => x.PublicId == id)
                   .FirstOrDefaultAsync(ct)
               ?? throw new NotFoundException(MessageCode.Reward.MissionNotFound);
    }
    
    public async Task AddAsync(Mission entity, CancellationToken ct = default)
    {
        await dbContext.Missions.AddAsync(entity, ct);
    }
}