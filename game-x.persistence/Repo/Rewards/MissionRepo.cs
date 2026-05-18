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
    public async Task<MissionDto[]> GetListAsync(CancellationToken ct = default)
    {
        return await dbContext.Missions
            .AsNoTracking()
            .ProjectToType<MissionDto>()
            .ToArrayAsync(ct);
    }
    
    public async Task<MissionDto> GetDetailAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Missions
            .AsNoTracking()
            .Where(x => x.PublicId == id)
            .ProjectToType<MissionDto>()
            .FirstOrDefaultAsync(ct)
            ?? throw new NotFoundException(MessageCode.Reward.MissionNotFound);
    }
    
    public async Task AddAsync(Mission entity, CancellationToken ct = default)
    {
        await dbContext.Missions.AddAsync(entity, ct);
    }
}