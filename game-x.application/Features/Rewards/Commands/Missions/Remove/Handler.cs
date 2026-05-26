using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.Missions.Remove;

public sealed class RemoveMissionHandler(
    IUnitOfWork unitOfWork,
    IMissionRepo repo,
    IMissionCacheService cache,
    ILogger<RemoveMissionHandler> logger
    ) : ICommandHandler<RemoveMissionCommand, Unit>
{
    public async Task<Unit> Handle(RemoveMissionCommand cmd, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                await repo.RemoveAsync(cmd.Id, ct);
                await unitOfWork.CommitAsync(ct);
                await cache.RefreshCache(ct);
                cache.RemoveGetDetailByAdmin(cmd.Id);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to remove mission.");
                throw new BadRequestException("Failed to remove mission.", e);
            }
        }, ct);
        
        return Unit.Value;
    }
}