using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.DeleteGameRecommend;

public sealed class DeleteGameRecommendHandler(
    IUnitOfWork unitOfWork,
    IGameRecommendRepo gameRecommendRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<DeleteGameRecommendCommand>
{
    public async Task<Unit> Handle(DeleteGameRecommendCommand request, CancellationToken ct = default)
    {
        await gameRecommendRepo.DeleteAsync(request.Id, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // Refresh cache data after database updated
        await gameProviderCache.RefreshGameRecommendListAsync(ct);

        return Unit.Value;
    }
}
