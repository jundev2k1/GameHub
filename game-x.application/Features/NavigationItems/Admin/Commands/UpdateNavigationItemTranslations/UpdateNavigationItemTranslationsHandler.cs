using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.NavigationItems.Admin.Commands.UpdateNavigationItemTranslations;

public sealed class UpdateNavigationItemTranslationsHandler(
    IUnitOfWork unitOfWork,
    INavigationItemRepo navigationItemRepo,
    INavigationCacheService navigationCache) : ICommandHandler<UpdateNavigationItemTranslationsCommand>
{
    public async Task<Unit> Handle(UpdateNavigationItemTranslationsCommand request, CancellationToken ct = default)
    {
        // Handling update translations
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await navigationItemRepo.UpdateTranslationAsync(request.Id, category =>
            {
                foreach (var translation in request.Translations)
                    category.UpsertTranslation(
                        LanguageCode.Of(translation.LanguageCode),
                        translation.Title.Trim());
            }, ct);
        }, ct);

        // Refreshing navigation items cache
        await navigationCache.RefreshNavigationItemsAsync(ct);

        return Unit.Value;
    }
}
