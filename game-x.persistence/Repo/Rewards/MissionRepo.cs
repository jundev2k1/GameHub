using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Exceptions;
using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Constants;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;
using Mapster;

namespace game_x.persistence.Repo.Rewards;

public sealed class MissionRepo(GameXContext dbContext) : IMissionRepo, IRepository
{
    public async Task<MissionListedAdminDto[]> GetAllForAdminAsync(CancellationToken ct = default)
    {
        return await dbContext.Missions
            .AsNoTracking()
            .ProjectToType<MissionListedAdminDto>()
            .ToArrayAsync(ct);
    }
    
    public async Task<MissionListedUserDto[]> GetAllForUserAsync(CancellationToken ct = default)
    {
        return await dbContext.Missions
            .AsNoTracking()
            .Where(x => x.IsActive)
            .ProjectToType<MissionListedUserDto>()
            .ToArrayAsync(ct);
    }
    
    public async Task<MissionDto> GetDetailForAdminAsync(Guid id, CancellationToken ct = default)
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
                MissionRewards = m.MissionRewards
                    .Select(mr => new
                    {
                        Reward = mr,
                        Definition = mr.RewardDefinition,
                        Catalog = mr.RewardDefinition != null ? mr.RewardDefinition.CatalogItem : null,
                    })
                    .Select(mr => new MissionRewardDto
                {
                    Id = mr.Reward.PublicId,
                    Sequence = mr.Reward.Sequence,
                    SortOrder = mr.Reward.SortOrder,
                    RequiredProgress = mr.Reward.RequiredProgress,
                    IsActive = mr.Reward.IsActive,
                    StartAt = mr.Reward.StartAt,
                    EndAt = mr.Reward.EndAt,
                    
                    RewardDefinitionId = mr.Definition!.PublicId,
                    Amount = mr.Definition.Amount,
                    Title = mr.Definition.Title,
                    Description = mr.Definition.Description,
                    RewardType = mr.Definition.Type,
                    
                    ItemId = mr.Catalog!.PublicId,
                    ItemName = mr.Catalog.Name,
                    ItemIconType = mr.Catalog.IconType,
                    ItemIcon = mr.Catalog.Icon
                }).ToArray()
            })
            .FirstOrDefaultAsync(ct);
            
            if(mission == null)
            throw new NotFoundException(MessageCode.Reward.MissionNotFound);
    
            return mission;
    }
    
    public async Task<MissionDto> GetDetailForUserAsync(string userId, Guid missionId, CancellationToken ct = default)
    {
        var mission = await dbContext.Missions
            .AsNoTracking()
            .Where(m => m.PublicId == missionId && m.IsActive)
            .SelectMany(
                m => m.UserMissions.Where(um => um.UserId == userId),
                (m, um) => new { Mission = m, UserMission = um }
            )
            .Select(x => new MissionDto
            {
                Id = x.Mission.PublicId,
                Code = x.Mission.Code,
                Type = x.Mission.Type,
                Title = x.Mission.Title,
                Description = x.Mission.Description,
                ResetType = x.Mission.ResetType,
                IsActive = x.Mission.IsActive,
                Progress = x.UserMission.Progress,
                Streak = x.UserMission.Streak,
                Status = x.UserMission.Status,
                LastProgressAt = x.UserMission.LastProgressAt,
                ConfigData = x.Mission.ConfigData,
                MissionRewards = x.Mission.MissionRewards
                    .Where(mr => mr.IsActive)
                    .Select(mr => new
                    {
                        Reward = mr,
                        Definition = mr.RewardDefinition,
                        Catalog = mr.RewardDefinition != null ? mr.RewardDefinition.CatalogItem : null,
                        Claim = mr.UserMissionClaims
                            .FirstOrDefault(uc => 
                                uc.UserId == userId && 
                                uc.CycleNumber == x.UserMission.CycleNumber)
                    })
                    .Select(r => new MissionRewardDto
                {
                    Id = r.Reward.PublicId,
                    IsActive = r.Reward.IsActive,
                    
                    RewardDefinitionId = r.Definition!.PublicId,
                    Amount = r.Definition.Amount,
                    Title = r.Definition.Title,
                    Description = r.Definition.Description,
                    RewardType = r.Definition.Type,
                    
                    ItemId = r.Catalog!.PublicId,
                    ItemName = r.Catalog.Name,
                    ItemIconType = r.Catalog.IconType,
                    ItemIcon = r.Catalog.Icon,
                    
                    ClaimId = r.Claim != null && r.Claim.Status == UserMissionClaimStatus.Available ? r.Claim.PublicId : null,
                    IsClaimed = r.Claim != null && r.Claim.Status == UserMissionClaimStatus.Claimed
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
    
    public async Task<bool> CodeExistsAsync(string code, CancellationToken ct = default)
    {
        return await dbContext.Missions.AnyAsync(x => x.Code == code, ct);
    }
    
    public async Task<IReadOnlyCollection<Mission>> GetTriggeredByEventAsync(
        UserEventType eventType,
        CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        var missions = await dbContext.Missions
            .AsNoTracking()
            .Where(x =>
                x.IsActive &&
                (x.StartAt == null || x.StartAt <= now) &&
                (x.EndAt == null || x.EndAt >= now))
            .ToListAsync(ct);

        return missions
            .Where(x => x.TriggerEvents.Contains(eventType))
            .ToList();
    }
    
    public async Task AddAsync(Mission entity, CancellationToken ct = default)
    {
        await dbContext.Missions.AddAsync(entity, ct);
    }
    
    public async Task UpdateAsync(Guid id, Action<Mission> updateAction, CancellationToken ct = default)
    {
        var entity = await dbContext.Missions
                         .FirstOrDefaultAsync(c => c.PublicId == id, ct)
                     ?? throw new NotFoundException(MessageCode.Reward.RewardPoolNotFound);

        updateAction.Invoke(entity);
    }

    public async Task RemoveAsync(Guid id, CancellationToken ct = default)
    {
        var mission = await dbContext.Missions.FirstOrDefaultAsync(x => x.PublicId == id, ct);
        if (mission is null)
            throw new NotFoundException(MessageCode.Reward.MissionNotFound);
        
        dbContext.Missions.Remove(mission);
    }
}