using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.CreateGameCategory;

public sealed class CreateGameCategoryHandler(
    IUnitOfWork unitOfWork,
    IGameCategoryRepo gameCategoryRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<CreateGameCategoryCommand>
{
    public async Task<Unit> Handle(CreateGameCategoryCommand request, CancellationToken ct = default)
    {
        var gameCategory = GameCategory.Create(
            request.Name,
            request.Description,
            request.Note,
            request.Priority);
        await gameCategoryRepo.AddAsync(gameCategory, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // Refresh cache data after database updated
        await gameProviderCache.RefreshGameCategoryList();

        return Unit.Value;
    }
}
