using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGamePlatformTranslations;

public sealed class UpdateGamePlatformTranslationsHandler(
    IUnitOfWork unitOfWork,
    IGamePlatformRepo gamePlatformRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<UpdateGamePlatformTranslationsCommand>
{
    public async Task<Unit> Handle(UpdateGamePlatformTranslationsCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await gamePlatformRepo.UpdateTranslationAsync(request.GamePlatformId, platform =>
            {
                foreach (var translation in request.Translations)
                    platform.UpsertTranslation(
                        LanguageCode.Of(translation.LanguageCode),
                        translation.Name.Trim(),
                        translation.Description.Trim(),
                        translation.Notes.Trim());
            }, ct);
        }, ct);

        await gameProviderCache.RefreshGamePlatformListAsync(ct);

        return Unit.Value;
    }
}
