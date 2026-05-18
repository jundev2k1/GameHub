using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGameTagTranslations;

public sealed class UpdateGameTagTranslationsHandler(
    IUnitOfWork unitOfWork,
    IGameTagRepo gameTagRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<UpdateGameTagTranslationsCommand>
{
    public async Task<Unit> Handle(UpdateGameTagTranslationsCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await gameTagRepo.UpdateTranslationAsync(request.GameTagId, tag =>
            {
                foreach (var translation in request.Translations)
                    tag.UpsertTranslation(
                        LanguageCode.Of(translation.LanguageCode),
                        translation.Name.Trim(),
                        translation.Description.Trim(),
                        translation.Notes.Trim());
            }, ct);
        }, ct);

        await gameProviderCache.RefreshGameTagListAsync(ct);

        return Unit.Value;
    }
}
