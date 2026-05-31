using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.NavigationItems.Admin.Commands.UpdateNavigationItemStatus;

public sealed class UpdateNavigationItemStatusHandler(
    IUnitOfWork unitOfWork,
    INavigationItemRepo navigationItemRepo,
    INavigationCacheService navigationCache) : ICommandHandler<UpdateNavigationItemStatusCommand>
{
    public async Task<Unit> Handle(UpdateNavigationItemStatusCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await navigationItemRepo.UpdateAsync(
                request.Id,
                item => item.UpdateStatus(request.Status),
                null,
                ct);
        }, ct);

        // Refreshing navigation items cache
        await navigationCache.RefreshNavigationItemsAsync(ct);

        return Unit.Value;
    }
}
