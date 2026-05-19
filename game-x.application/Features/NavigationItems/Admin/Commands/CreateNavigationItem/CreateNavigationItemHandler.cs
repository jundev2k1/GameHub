using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.NavigationItems.Admin.Commands.CreateNavigationItem;

public sealed class CreateNavigationItemHandler(
    IUnitOfWork unitOfWork,
    INavigationItemRepo navigationItemRepo,
    INavigationCacheService navigationCacheService,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<CreateNavigationItemCommand>
{
    public async Task<Unit> Handle(CreateNavigationItemCommand request, CancellationToken ct = default)
    {
        // Check and get local id for creating
        int? categoryId = null;
        if (request.TargetType is NavigationTargetType.Category && request.TargetId.HasValue)
        {
            var targetCategory = gameProviderCache.CategoryList.FirstOrDefault(c => c.Id == request.TargetId)
                ?? throw new BadRequestException(MessageCode.System.ResourceNotFound, new { cateId = request.TargetId });

            categoryId = targetCategory.LocalId;
        }

        // Handle creating new item
        await unitOfWork.WithTransactionAsync(async () =>
        {
            var navigationItem = NavigationItem.Create(
                request.Title.Trim(),
                request.Slug.Trim(),
                request.TargetType,
                categoryId,
                request.CustomUrl.Trim(),
                request.Priority);
            await navigationItemRepo.CreateAsync(navigationItem, ct);
        }, ct);

        // Refreshing navigation items cache
        await navigationCacheService.RefreshNavigationItemsAsync(ct);

        return Unit.Value;
    }
}
