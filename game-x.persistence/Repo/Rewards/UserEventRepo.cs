using game_x.application.Common.Abstractions;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Exceptions;
using game_x.domain.Constants;
using game_x.domain.Entities.Rewards;

namespace game_x.persistence.Repo.Rewards;

public sealed class UserEventRepo(GameXContext dbContext) : IUserEventRepo, IRepository
{
    public async Task<UserEvent?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.UserEvents
                   .AsNoTracking()
                   .FirstOrDefaultAsync(x => x.PublicId == id, ct);
    }
    
    public async Task AddAsync(UserEvent entity, CancellationToken ct = default)
    {
        await dbContext.UserEvents.AddAsync(entity, ct);
    }
    
    public async Task UpdateAsync(Guid id, Action<UserEvent> updateAction, CancellationToken ct = default)
    {
        var entity = await dbContext.UserEvents
                         .FirstOrDefaultAsync(c => c.PublicId == id, ct)
                     ?? throw new NotFoundException(MessageCode.Reward.UserEventNotFound);

        updateAction.Invoke(entity);
    }
}