using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Games.Admin.Commands.UpdateGameCategoryTranslations;

public sealed class UpdateGameCategoryTranslationsHandler(
    IUnitOfWork unitOfWork,
    IGameCategoryRepo gameCategoryRepo,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<UpdateGameCategoryTranslationsCommand>
{
    public async Task<Unit> Handle(UpdateGameCategoryTranslationsCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await gameCategoryRepo.UpdateTranslationAsync(request.GameCateId, category =>
            {
                foreach (var translation in request.Translations)
                    category.UpsertTranslation(
                        LanguageCode.Of(translation.LanguageCode),
                        translation.Name.Trim(),
                        translation.Description.Trim(),
                        translation.Notes.Trim());
            }, ct);
        }, ct);

        await gameProviderCache.RefreshGameCategoryListAsync(ct);

        return Unit.Value;
    }
}
