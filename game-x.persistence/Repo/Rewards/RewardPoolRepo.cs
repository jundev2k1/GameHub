using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Exceptions;
using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Constants;
using game_x.domain.Entities.Rewards;
using Mapster;

namespace game_x.persistence.Repo.Rewards;

public sealed class RewardPoolRepo(GameXContext dbContext) : IRewardPoolRepo, IRepository
{
    public async Task<RewardPoolDto[]> GetListAsync(CancellationToken ct = default)
    {
        return await dbContext.RewardPools
            .AsNoTracking()
            .ProjectToType<RewardPoolDto>()
            .ToArrayAsync(ct);
    }
    
    public async Task<RewardPool> GetDetailByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.RewardPools
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.PublicId == id, ct)
            ?? throw new BadRequestException(MessageCode.Reward.RewardPoolNotFound);
    }
}