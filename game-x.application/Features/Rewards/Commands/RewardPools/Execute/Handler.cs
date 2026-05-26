using game_x.application.Contract.Infrastructure.Caching.Rewards;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Contract.Persistence.Repo.Reward;
using game_x.application.Events.Account.OnUserBalanceUpdated;
using game_x.application.Events.Rewards.OnUserInventoryUpdated;
using game_x.application.Features.Rewards.Dtos;
using game_x.domain.Entities.Rewards;
using game_x.domain.Enum.Rewards;
using Microsoft.Extensions.Logging;

namespace game_x.application.Features.Rewards.Commands.RewardPools.Execute;

public sealed class RewardPoolExecuteHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IRewardPoolRepo poolRepo,
    IRewardPoolItemRepo itemRepo,
    IExecutionRepo executionRepo,
    IUserRewardRepo userRewardRepo,
    IUserInventoryRepo userInventoryRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    ITransactionRepo transactionRepo,
    IUserBalanceRepo userBalanceRepo,
    IRewardPoolItemCacheService itemCache,
    IUserInventoryCacheService userInventoryCache,
    ICatalogItemCacheService catalogItemCache,
    IApplicationEventDispatcher dispatcher,
    ILogger<RewardPoolExecuteHandler> logger
) : ICommandHandler<RewardPoolExecuteCommand, RewardPoolExecuteResponse>
{
    public async Task<RewardPoolExecuteResponse> Handle(
        RewardPoolExecuteCommand request,
        CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var (rewardPool, rewardItems) = await Validate(userId, request, ct);
        
        var selectedItem = SelectReward(rewardItems ?? []);

        var poolItem = await itemRepo.GetDetailByIdAsync(selectedItem.Id, ct);
        if (!poolItem.IsActive)
            throw new BadRequestException(MessageCode.Reward.RewardPoolItemInactive);
        
        if(poolItem.RewardDefinition == null)
            throw new BadRequestException(MessageCode.Reward.RewardDefinitionNotFound);
        
        var increasedItem = poolItem.RewardDefinition.CatalogItemId != null ? 
            await userInventoryRepo.GetDetailAsync(userId, (int)poolItem.RewardDefinition.CatalogItemId!, ct)
            : null;
        RewardPoolExecuteResponse response = null!;

        await unitOfWork.WithTransactionAsync(async () =>
        {
            try
            {
                var execution = Execution.Create(
                    userId: userId,
                    type: ExecutionType.Spin,
                    rewardPoolId: rewardPool.Id
                );
                await executionRepo.AddAsync(execution, ct);

                Transaction? transaction = null;

                switch (selectedItem.RewardType)
                {
                    case RewardItemType.Balance:
                    {
                        if(selectedItem.Amount > 0)
                            transaction = await HandleBalanceReward(userId, selectedItem.Amount.Value, ct);
                        break;
                    }
                    case RewardItemType.CatalogItem:
                    {
                        await UpdateInventory(userId, poolItem, increasedItem, ct);
                        break;
                    }
                }

                var userReward = UserReward.Create(
                    userId: userId,
                    execution: execution,
                    rewardPoolItemId: poolItem.Id,
                    rewardDefinitionId: poolItem.RewardDefinitionId,
                    rewardType: poolItem.RewardDefinition?.Type ?? RewardItemType.None,
                    title: poolItem.RewardDefinition?.Title,
                    amount: poolItem.RewardDefinition?.Amount ?? 0,
                    transaction: transaction
                );
                
                await userRewardRepo.AddAsync(userReward, ct);
   
                execution.MarkSuccess();

                await unitOfWork.CommitAsync(ct);

                switch (selectedItem.RewardType)
                {
                    case RewardItemType.Balance:
                    {
                        await dispatcher.Publish(new OnUserBalanceUpdatedEvent(userId), ct);
                        break;
                    }
                    case RewardItemType.CatalogItem:
                    {
                        await userInventoryCache.RefreshCache(userId, ct);
                        var inventories = await userInventoryCache.GetAll(userId, ct);
                        await dispatcher.Publish(new OnUserInventoryUpdatedEvent(userId, inventories ?? []), ct);
                        break;
                    }
                }
                
                response = new RewardPoolExecuteResponse
                {
                    RewardId = poolItem.RewardDefinition!.PublicId,
                    RewardCode = poolItem.RewardDefinition!.Code,
                    RewardTitle = poolItem.RewardDefinition.Title,
                    RewardType = poolItem.RewardDefinition.Type,
                    Amount = poolItem.RewardDefinition?.Amount
                };
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to spin reward.");
                throw new BadRequestException("Failed to spin reward.", e);
            }
        }, ct);

        return response;
    }

    private async Task<(RewardPool rewardPool, RewardPoolItemDto[]? rewardItems)> 
        Validate(string userId, RewardPoolExecuteCommand request, CancellationToken ct = default)
    {
        var rewardPool = await poolRepo.GetDetailByIdAsync(request.RewardPoolPublicId, ct);
        if (rewardPool is null) throw new NotFoundException(MessageCode.Reward.RewardPoolNotFound);
        if (!rewardPool.IsActive) throw new BadRequestException(MessageCode.Reward.RewardPoolInactive);

        var rewardItems = await itemCache.GetAllByAdmin(rewardPool.PublicId, ct);
        if (rewardItems == null || rewardItems.Length == 0)
            throw new BadRequestException("No reward items available.");

        if (rewardPool.Config?.RequiredItemType == null)
            throw new BadRequestException(MessageCode.Reward.ItemRequiredInPool);

        var catalogItems = await catalogItemCache.GetAll(ct);

        var requiredCatalogItem = rewardPool.Config.RequiredCatalogItemId != null 
            ? catalogItems?.FirstOrDefault(x => x.Id == rewardPool.Config.RequiredCatalogItemId)
            : catalogItems?.FirstOrDefault(ci => ci.Category == rewardPool.Config.RequiredItemType);
        
        if (requiredCatalogItem == null)
            throw new BadRequestException(MessageCode.Reward.ItemRequiredInPool);
        
        var decreasedItem = await userInventoryRepo.GetDetailAsync(userId, requiredCatalogItem.Id, ct);
        if(decreasedItem == null || decreasedItem.Quantity < rewardPool.Config.RequiredItemAmount)
            throw  new BadRequestException(MessageCode.Reward.ItemInsufficient);
        
        decreasedItem.Deduct(rewardPool.Config.RequiredItemAmount);
        return (rewardPool, rewardItems);
    }
    
    private static RewardPoolItemDto SelectReward(IReadOnlyList<RewardPoolItemDto> items)
    {
        var totalWeight = items.Sum(x => x.Weight);
        var random = Random.Shared.Next(1, totalWeight + 1);
        var cumulative = 0;

        foreach (var item in items)
        {
            cumulative += item.Weight;
            if (random <= cumulative) return item;
        }

        return items.Last();
    }

    private async Task UpdateInventory(string userId, RewardPoolItem poolItem, UserInventory? userInventory, CancellationToken ct = default)
    {
        if (poolItem.RewardDefinition?.CatalogItemId != null)
            if (userInventory != null) userInventory.Add((int?)poolItem.RewardDefinition?.Amount ?? 0);
            else
            {
                var inventory = UserInventory.Create(
                    userId, 
                    (int)poolItem.RewardDefinition.CatalogItemId, 
                    (int?)poolItem.RewardDefinition?.Amount ?? 0);
                        
                await userInventoryRepo.AddAsync(inventory, ct);
            }
    }
    
    private async Task<Transaction> HandleBalanceReward(string userId, decimal amount, CancellationToken ct)
    {
        var token = await cryptoTokenRepo.GetBySymbolAndNetworkAsync(CryptoTokenSymbol.Usdt, NetworkType.Tron, ct);
        if (token == null || token.Status != CryptoTokenStatus.Active)
            throw new BadRequestException(MessageCode.Crypto.CryptoTokenUnsupported);
        
        var balance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, token.Id, ct)
                      ?? throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);
        
        var tx = Transaction.Create(
            type: TransactionType.Reward,
            userId: userId,
            amount: amount,
            cryptoTokenId: token.Id);
        
        var internalTx = TransactionInternal.Create(
            sourceType: TransactionSourceType.Reward);
        tx.AddTxInternal(internalTx);
        await transactionRepo.AddAsync(tx, ct);
        await userBalanceRepo.UpdateAsync(balance.PublicId, x => { x.AdjustAmount(amount, true); }, ct);
        await unitOfWork.SaveChangesAsync(ct);
        tx.Confirm(amount, balance.Amount);
        return tx;
    }
}