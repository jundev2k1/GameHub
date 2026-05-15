using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Exceptions;
using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Constants;
using game_x.domain.Entities.Rewards;
using Mapster;

namespace game_x.persistence.Repo.Rewards;

public sealed class RewardPoolItemRepo(GameXContext dbContext) : IRewardPoolItemRepo, IRepository
{
    public async Task<RewardPoolItemDto[]> GetListAsync(int poolId, CancellationToken ct = default)
    {
        return await dbContext.RewardPoolItems
            .AsNoTracking()
            .OrderByDescending(x => x.SortOrder)
            .Where(x => x.RewardPoolId == poolId)
            .ProjectToType<RewardPoolItemDto>()
            .ToArrayAsync(ct);
    }
    
    public async Task<RewardPoolItem> GetDetailByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.RewardPoolItems
            .AsNoTracking()
            .Include(x => x.RewardDefinition)
            .FirstOrDefaultAsync(x => x.PublicId == id, ct)
            ?? throw new BadRequestException(MessageCode.Reward.RewardPoolItemNotFound);
    }
    
    public async Task AddAsync(RewardPoolItem entity, CancellationToken ct = default)
    {
        await dbContext.RewardPoolItems.AddAsync(entity, ct);
    }
}