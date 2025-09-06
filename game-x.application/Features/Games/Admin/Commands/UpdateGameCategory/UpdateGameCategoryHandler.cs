using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGameCategory;

public sealed class UpdateGameCategoryHandler(
    IUnitOfWork unitOfWork,
    IGameCategoryRepo gameCategoryRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<UpdateGameCategoryCommand>
{
    public async Task<Unit> Handle(UpdateGameCategoryCommand request, CancellationToken ct = default)
    {
        await gameCategoryRepo.UpdateAsync(request.Id, async category =>
        {
            category.Update(
                request.Name,
                request.Description,
                request.Note, request.Priority,
                request.IsActive);
            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        // Refresh cache data after database updated
        await gameProviderCache.RefreshGameCategoryList();
        await gameProviderCache.RefreshGameList();

        return Unit.Value;
    }
}
