using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.NavigationItems.Admin.Commands.DeleteNavigationItem;

public sealed class DeleteNavigationItemHandler(
    IUnitOfWork unitOfWork,
    INavigationItemRepo navigationItemRepo,
    INavigationCacheService navigationCache) : ICommandHandler<DeleteNavigationItemCommand>
{
    public async Task<Unit> Handle(DeleteNavigationItemCommand request, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await navigationItemRepo.DeleteAsync(request.Id, ct);
        }, ct);

        await navigationCache.RefreshNavigationItemsAsync(ct);

        return Unit.Value;
    }
}
