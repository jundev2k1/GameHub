using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGameTypeTranslations;

public sealed class UpdateGameTypeTranslationsHandler(
    IUnitOfWork unitOfWork,
    IGameTypeRepo gameTypeRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<UpdateGameTypeTranslationsCommand>
{
    public async Task<Unit> Handle(UpdateGameTypeTranslationsCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await gameTypeRepo.UpdateTranslationAsync(request.GameTypeId, type =>
            {
                foreach (var translation in request.Translations)
                    type.UpsertTranslation(
                        LanguageCode.Of(translation.LanguageCode),
                        translation.Name.Trim(),
                        translation.Description.Trim(),
                        translation.Notes.Trim());
            }, ct);
        }, ct);

        await gameProviderCache.RefreshGameTypeListAsync(ct);

        return Unit.Value;
    }
}
