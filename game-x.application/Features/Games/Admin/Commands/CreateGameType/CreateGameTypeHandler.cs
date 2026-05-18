using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.CreateGameType;

public sealed class CreateGameTypeHandler(
    IUnitOfWork unitOfWork,
    IGameTypeRepo gameTypeRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<CreateGameTypeCommand>
{
    public async Task<Unit> Handle(CreateGameTypeCommand request, CancellationToken ct = default)
    {
        var gameCategory = GameType.Create(
            request.Name,
            request.Description,
            request.Note,
            request.Priority);
        await gameTypeRepo.AddAsync(gameCategory, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // Refresh cache data after database updated
        await gameProviderCache.RefreshGameTypeListAsync(ct);

        return Unit.Value;
    }
}
