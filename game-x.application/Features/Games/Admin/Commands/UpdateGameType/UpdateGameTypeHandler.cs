using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGameType;

public sealed class UpdateGameTypeHandler(
    IUnitOfWork unitOfWork,
    IGameTypeRepo gameTypeRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<UpdateGameTypeCommand>
{
    public async Task<Unit> Handle(UpdateGameTypeCommand request, CancellationToken ct = default)
    {
        await gameTypeRepo.UpdateAsync(request.Id, async type =>
        {
            type.Update(
                request.Name,
                request.Description,
                request.Note,
                request.Priority,
                request.IsActive);
            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        // Refresh cache data after database updated
        await gameProviderCache.RefreshGameTypeListAsync(ct);
        await gameProviderCache.RefreshGameListAsync(ct);

        return Unit.Value;
    }
}
