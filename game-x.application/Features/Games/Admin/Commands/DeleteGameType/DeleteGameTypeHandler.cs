using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.DeleteGameType;

public sealed class DeleteGameTypeHandler(
    IUnitOfWork unitOfWork,
    IGameTypeRepo gameTypeRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<DeleteGameTypeCommand>
{
    public async Task<Unit> Handle(DeleteGameTypeCommand request, CancellationToken ct = default)
    {
        await gameTypeRepo.DeleteAsync(request.Id, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // Refresh cache data after database updated
        await gameProviderCache.RefreshGameTypeList();
        await gameProviderCache.RefreshGameList();

        return Unit.Value;
    }
}
