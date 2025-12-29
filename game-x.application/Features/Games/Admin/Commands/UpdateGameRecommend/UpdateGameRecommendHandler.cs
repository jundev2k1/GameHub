using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGameRecommend;

public sealed class UpdateGameRecommendHandler(
    IUnitOfWork unitOfWork,
    IGameRecommendRepo gameRecommendRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<UpdateGameRecommendCommand>
{
    public async Task<Unit> Handle(UpdateGameRecommendCommand request, CancellationToken ct = default)
    {
        var gameList = gameProviderCache.GameList;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await gameRecommendRepo.UpdateAsync(request.Id!.Value, async recommend =>
            {
                // Check overlap time
                var overlapItem = await gameRecommendRepo.GetOverlapItemAsync(recommend, ct);
                if (overlapItem != null) throw new BadRequestException(
                    MessageCode.System.TimeOverlap,
                    new
                    {
                        id = overlapItem.PublicId,
                        startDate = overlapItem.StartDate ?? DateTime.MinValue,
                        endDate = overlapItem.EndDate ?? DateTime.MaxValue,
                    });

                // Execute update
                recommend.Update(
                    request.Name,
                    request.Description,
                    request.StartDate,
                    request.EndDate);
                var recommendItems = request.Items
                    .Select(item =>
                    {
                        var targetGame = gameList.FirstOrDefault(g => g.Id == item.GameId)
                            ?? throw new NotFoundException("Game item is not exists.");
                        var recommendItem = GameRecommendItem.Create(
                            recommend.Id,
                            targetGame.LocalId,
                            item.Priority,
                            item.CustomTitle,
                            item.IsActive);
                        return recommendItem;
                    })
                    .ToList();

                // Delete all of current recommend items
                await gameRecommendRepo.DeleteAllItemsAsync(request.Id.Value, ct);
                await unitOfWork.SaveChangesAsync(ct);

                // Add new recommend items
                await gameRecommendRepo.AddItemsAsync(request.Id.Value, recommendItems, ct);
            });
        }, ct);

        // Refresh cache data after database updated
        await gameProviderCache.RefreshGameRecommendList();

        return Unit.Value;
    }
}
