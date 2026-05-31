using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.CreateGameTag;

public sealed class CreateGameTagHandler(
    IUnitOfWork unitOfWork,
    IGameTagRepo gameTagRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<CreateGameTagCommand>
{
    public async Task<Unit> Handle(CreateGameTagCommand request, CancellationToken ct = default)
    {
        var gameTag = GameTag.Create(
            request.Name,
            request.Description,
            GameTagIcon.Of(request.Icon),
            GameTagColor.Of(request.Color),
            request.Note);
        await gameTagRepo.AddAsync(gameTag, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // Refresh cache data after database updated
        await gameProviderCache.RefreshGameTagListAsync(ct);

        return Unit.Value;
    }
}
