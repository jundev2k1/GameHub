using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.CreateGameRecommend;

public sealed class CreateGameRecommendHandler(
    IUnitOfWork unitOfWork,
    IGameRecommendRepo gameRecommendRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<CreateGameRecommendCommand>
{
    public async Task<Unit> Handle(CreateGameRecommendCommand request, CancellationToken ct = default)
    {
        var gameRecommend = GameRecommend.Create(
            request.Name,
            request.Description,
            request.Status,
            request.StartDate,
            request.EndDate);
        var gameList = gameProviderCache.GameList;
        var reqGameIds = request.Items.Select(i => i.GameId);

        var overlapItem = await gameRecommendRepo.GetOverlapItemAsync(gameRecommend, ct);
        if (overlapItem != null) throw new BadRequestException(
            MessageCode.System.TimeOverlap,
            new
            {
                id = overlapItem.PublicId,
                startDate = overlapItem.StartDate ?? DateTime.MinValue,
                endDate = overlapItem.EndDate ?? DateTime.MaxValue,
            });

        foreach (var item in request.Items)
        {
            var targetGame = gameList.FirstOrDefault(g => g.Id == item.GameId)
                ?? throw new NotFoundException(nameof(item.GameId), item.GameId);
            gameRecommend.AddGame(GameRecommendItem.Create(
                default,
                targetGame.LocalId,
                item.Priority,
                item.CustomTitle,
                item.IsActive));
        }
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await gameRecommendRepo.AddAsync(gameRecommend, ct);
        }, ct);

        // Refresh cache data after the database updated
        await gameProviderCache.RefreshGameRecommendList();

        return Unit.Value;
    }
}
