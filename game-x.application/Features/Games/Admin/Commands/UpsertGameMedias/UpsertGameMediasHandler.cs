using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;
using Microsoft.EntityFrameworkCore;

namespace game_x.application.Features.Games.Admin.Commands.UpsertGameMedias;

public sealed class UpsertGameMediasHandler(
    IUnitOfWork unitOfWork,
    IGameRepo gameRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<UpsertGameMediasCommand>
{
    public async Task<Unit> Handle(UpsertGameMediasCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await gameRepo.UpdateGameAsync(
                request.Id,
                query => query.Include(q => q.GameMedias),
                async game =>
                {
                    var gameMedias = request.Items
                        .Select(i => {
                            var existing = i.Id.HasValue
                                ? game.GameMedias.FirstOrDefault(gm => gm.PublicId == i.Id)
                                    ?? throw new BadRequestException(MessageCode.System.ResourceNotFound, new { nonExistId = i.Id })
                                : null;
                            return GameMedia.Create(game.Id, i.Id, existing?.FileId, i.Type, i.Category, i.Title.Trim(), i.Note?.Trim(), i.Priority);
                        })
                        .ToArray();
                    game.UpsertMediaFiles(gameMedias);
                    await Task.CompletedTask;
                }, ct);
        }, ct);

        // Handle refresh game media from in-memory cache
        await gameProviderCache.RefreshSpecifyGameMediaAsync(request.Id, null, ct);

        return Unit.Value;
    }
}
