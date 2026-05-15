using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGameTranslations;

public sealed class UpdateGameTranslationsHandler(
    IUnitOfWork unitOfWork,
    IGameRepo gameRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<UpdateGameTranslationsCommand>
{
    public async Task<Unit> Handle(UpdateGameTranslationsCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await gameRepo.UpdateGameTranslationAsync(request.GameId, game =>
            {
                foreach (var translation in request.Translations)
                    game.UpsertTranslation(
                        LanguageCode.Of(translation.LanguageCode),
                        translation.Name.Trim(),
                        translation.Description.Trim(),
                        translation.Notes.Trim());
            }, ct);
        }, ct);

        await gameProviderCache.RefreshGameListAsync(ct);

        return Unit.Value;
    }
}
