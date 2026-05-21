using System.Linq.Expressions;
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
    
    public async Task AddRangeAsync(IEnumerable<RewardPoolItem> items, CancellationToken ct = default)
    {
        await dbContext.RewardPoolItems.AddRangeAsync(items, ct);
    }
    
    public async Task BulkDeleteAsync(Expression<Func<RewardPoolItem, bool>> predicate, CancellationToken ct = default)
    {
        await dbContext.RewardPoolItems.Where(predicate).ExecuteDeleteAsync(ct);
    }
}