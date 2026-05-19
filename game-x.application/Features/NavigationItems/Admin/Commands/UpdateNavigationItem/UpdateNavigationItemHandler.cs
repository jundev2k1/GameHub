using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.NavigationItems.Admin.Commands.UpdateNavigationItem;

public sealed class UpdateNavigationItemHandler(
    IUnitOfWork unitOfWork,
    INavigationItemRepo navigationItemRepo,
    INavigationCacheService navigationCacheService,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<UpdateNavigationItemCommand>
{
    public async Task<Unit> Handle(UpdateNavigationItemCommand request, CancellationToken ct = default)
    {
        // Check and get local id for updating
        int? categoryId = null;
        if (request.TargetType is NavigationTargetType.Category && request.TargetId.HasValue)
        {
            var targetCategory = gameProviderCache.CategoryList.FirstOrDefault(c => c.Id == request.TargetId)
                ?? throw new BadRequestException(MessageCode.System.ResourceNotFound, new { cateId = request.TargetId });

            categoryId = targetCategory.LocalId;
        }

        // Handle updating navigation item
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await navigationItemRepo.UpdateAsync(
                request.Id,
                item =>
                {
                    item.Update(
                        request.Title.Trim(),
                        request.Slug.Trim(),
                        request.TargetType,
                        categoryId,
                        request.CustomUrl.Trim(),
                        request.Priority,
                        request.IsActive);
                },
                null,
                ct);
        }, ct);

        // Refreshing navigation items cache
        await navigationCacheService.RefreshNavigationItemsAsync(ct);

        return Unit.Value;
    }
}
