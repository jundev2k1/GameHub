using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.domain.Entities.Rewards;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.AddUserCatalogItem;

public sealed class AddUserCatalogItemHandler(
    IUnitOfWork unitOfWork,
    ICatalogItemRepo catalogItemRepo,
    IUserRepo userRepo,
    IUserInventoryCacheService cache,
    IUserInventoryRepo userInventoryRepo,
    ILogger<AddUserCatalogItemHandler> logger
    ) : ICommandHandler<AddUserCatalogItemCommand, Unit>
{
    public async Task<Unit> Handle(AddUserCatalogItemCommand request, CancellationToken ct = default)
    {
        var isExistedUser = await userRepo.IsExistUserIdAsync(request.UserId, ct);
        if (!isExistedUser)
            throw new NotFoundException(MessageCode.User.UserNotFound);
        
        var catalogItem = await catalogItemRepo.GetByCodeAsync(request.Code, ct);
        var userInventory = await userInventoryRepo.GetDetailAsync(request.UserId, catalogItem.Id, ct);
        
        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                if (userInventory != null) userInventory.Add(request.Quantity);
                else
                {
                    var inventory = UserInventory.Create(
                        request.UserId, 
                        catalogItem.Id, 
                        request.Quantity);
                        
                    await userInventoryRepo.AddAsync(inventory, ct);
                }
                await unitOfWork.SaveChangesAsync(ct);
                await cache.RefreshCache(request.UserId, ct);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to add user inventory.");
                throw new BadRequestException("Failed to add user inventory.", e);
            }
        }, ct);
        
        return Unit.Value;
    }
}