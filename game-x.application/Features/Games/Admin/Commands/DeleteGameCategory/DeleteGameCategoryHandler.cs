using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.DeleteGameCategory;

public sealed class DeleteGameCategoryHandler(
    IUnitOfWork unitOfWork,
    IGameCategoryRepo gameCategoryRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<DeleteGameCategoryCommand>
{
    public async Task<Unit> Handle(DeleteGameCategoryCommand request, CancellationToken ct = default)
    {
        await gameCategoryRepo.DeleteAsync(request.Id, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // Refresh cache data after database updated
        await gameProviderCache.RefreshGameCategoryListAsync(ct);
        await gameProviderCache.RefreshGameListAsync(ct);

        return Unit.Value;
    }
}
