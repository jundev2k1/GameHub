using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGamePlatform;

public sealed class UpdateGamePlatformHandler(
    IUnitOfWork unitOfWork,
    IGamePlatformRepo gamePlatformRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<UpdateGamePlatformCommand>
{
    public async Task<Unit> Handle(UpdateGamePlatformCommand request, CancellationToken ct = default)
    {
        await gamePlatformRepo.UpdateAsync(request.Id, async platform =>
        {
            platform.Update(
                request.Name,
                request.Description,
                request.Note,
                request.Priority,
                request.IsActive);
            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        // Refresh cache data after database updated
        await gameProviderCache.RefreshGamePlatformList();
        await gameProviderCache.RefreshGameList();

        return Unit.Value;
    }
}
