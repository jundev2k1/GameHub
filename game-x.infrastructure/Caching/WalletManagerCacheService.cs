using game_x.application.Common.Abstractions.Events;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Atg;
using game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.application.Contract.Infrastructure.ExternalApi.SasSlot;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnRefreshWalletFailed;
using game_x.application.Exceptions;
using game_x.application.Features.Accounts.User.Dtos;
using game_x.application.Features.Games.Dtos;
using game_x.share.Extensions;
using game_x.share.ExternalApi.Etl998.Dtos.Wallet;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class WalletManagerCacheService(
    IMemoryCache cache,
    IUserBalanceRepo userBalanceRepo,
    IUserRepo userRepo,
    IGameProviderCacheService gameProviderCache,
    IGameBaccaratService gameBaccaratService,
    IEtl998Service etl998Service,
    ISasSlotService sasSlotService,
    IGameProviderService gameProvider,
    IAtgService atgService,
    IApplicationEventDispatcher eventDispatcher,
    IAppLogger<WalletManagerCacheService> logger) : CacheService(cache), IWalletManagerCacheService
{
    private const string CacheKeyPrefix = "wallet-manager:";

    public async Task<UserWalletDto> GetWalletAsync(string userId)
    {
        var cacheKey = $"{CacheKeyPrefix}{userId}";
        var wallet = Get<UserWalletDto>(cacheKey);
        if (wallet is not null) return wallet;

        await RefreshWalletAsync(userId);
        return Get<UserWalletDto>(cacheKey)!;
    }

    public async Task<UserWalletExternalItemDto> GetExternalWalletAsync(string userId, Guid platformId)
    {
        var currentWallet = await GetWalletAsync(userId);
        return currentWallet.ExternalWallets
            .FirstOrDefault(ew => ew.PlatformId == platformId)
            ?? throw new NotFoundException(nameof(platformId), platformId);
    }

    public async Task<bool> IsExistExternalWalletAsync(string userId, Guid platformId)
    {
        var currentWallet = await GetWalletAsync(userId);
        return currentWallet.ExternalWallets
            .Any(ew => ew.PlatformId == platformId);
    }

    public async Task RefreshWalletAsync(string userId)
    {
        var cacheKey = $"{CacheKeyPrefix}{userId}";
        var targetUser = await userRepo.GetUserByIdAsync(userId);
        var userBalances = await userBalanceRepo.GetBalancesByUserIdAsync(userId);
        var userWallet = new UserWalletDto
        {
            InternalWallets = [.. userBalances.Select(ub => ub.Adapt<UserWalletInternalItemDto>())],
            ExternalWallets = [],
        };

        var g598Balance = await GetGame598WalletAsync(targetUser);
        await CreateOrUpdateExternalWalletAsync(
            userId: userId,
            wallet: userWallet,
            targetWallet: null,
            platform: gameProviderCache.G598Platform,
            balance: g598Balance);

        var baccaratBalance = await GetBaccaratWalletAsync(targetUser);
        await CreateOrUpdateExternalWalletAsync(
            userId: userId,
            wallet: userWallet,
            targetWallet: null,
            platform: gameProviderCache.BaccaratPlatform,
            balance: baccaratBalance);

        var elt998Balance = await GetElt998WalletAsync(targetUser);
        await CreateOrUpdateExternalWalletAsync(
            userId: userId,
            wallet: userWallet,
            targetWallet: null,
            platform: gameProviderCache.Etl998Platform,
            balance: elt998Balance);

        var sasSlotBalance = await GetSasSlotWalletAsync(targetUser);
        await CreateOrUpdateExternalWalletAsync(
            userId: userId,
            wallet: userWallet,
            targetWallet: null,
            platform: gameProviderCache.SasSlotPlatform,
            balance: sasSlotBalance);

        var atgBalance = await GetAtgWalletAsync(targetUser);
        await CreateOrUpdateExternalWalletAsync(
            userId: userId,
            wallet: userWallet,
            targetWallet: null,
            platform: gameProviderCache.AtgPlatform,
            balance: atgBalance);
        
        Set(cacheKey, userWallet);
    }

    public async Task RefreshInternalWalletAsync(string userId, Guid walletId, decimal amount, decimal frozenAmount)
    {
        var userWallet = await GetWalletAsync(userId);
        var targetWallet = userWallet.InternalWallets
            .FirstOrDefault(ub => ub.WalletId == walletId)
            ?? throw new BadRequestException();
        targetWallet.Amount = amount;
        targetWallet.FrozenAmount = frozenAmount;

        await Task.CompletedTask;
        var cacheKey = $"{CacheKeyPrefix}{userId}";
        Set(cacheKey, userWallet);
    }

    public async Task RefreshExternalWalletAsync(string userId, Guid platformId)
    {
        var targetUser = await userRepo.GetUserByIdAsync(userId);
        var userWallet = await GetWalletAsync(userId);
        var targetWallet = userWallet.ExternalWallets
            .FirstOrDefault(w => w.PlatformId == platformId);

        if (platformId == GameConstants.PLATFORM_ID_G598)
        {
            var balance = await GetGame598WalletAsync(targetUser);
            await CreateOrUpdateExternalWalletAsync(
                userId: userId,
                wallet: userWallet,
                targetWallet: targetWallet,
                platform: gameProviderCache.G598Platform,
                balance: balance);
        }

        if (platformId == GameConstants.PLATFORM_ID_GAMEBACCARAT)
        {
            var balance = await GetBaccaratWalletAsync(targetUser);
            await CreateOrUpdateExternalWalletAsync(
                userId: userId,
                wallet: userWallet,
                targetWallet: targetWallet,
                platform: gameProviderCache.BaccaratPlatform,
                balance: balance);
        }

        if (platformId == GameConstants.PLATFORM_ID_ETL998_GAMEBACCARAT)
        {
            var balance = await GetElt998WalletAsync(targetUser);
            await CreateOrUpdateExternalWalletAsync(
                userId: userId,
                wallet: userWallet,
                targetWallet: targetWallet,
                platform: gameProviderCache.Etl998Platform,
                balance: balance);
        }

        if (platformId == GameConstants.PLATFORM_ID_SASSLOT)
        {
            var balance = await GetSasSlotWalletAsync(targetUser);
            await CreateOrUpdateExternalWalletAsync(
                userId: userId,
                wallet: userWallet,
                targetWallet: targetWallet,
                platform: gameProviderCache.SasSlotPlatform,
                balance: balance);
        }

        if (platformId == GameConstants.PLATFORM_ID_ATG)
        {
            var balance = await GetAtgWalletAsync(targetUser);
            await CreateOrUpdateExternalWalletAsync(
                userId: userId,
                wallet: userWallet,
                targetWallet: targetWallet,
                platform: gameProviderCache.AtgPlatform,
                balance: balance);
        }

        var cacheKey = $"{CacheKeyPrefix}{userId}";
        Set(cacheKey, userWallet);
    }

    private async Task<decimal?> GetGame598WalletAsync(User targetUser)
    {
        if (targetUser.UserExtend is null
            || targetUser.UserExtend.GameProviderAccount.IsNullOrWhiteSpace()
            || targetUser.UserExtend.GameProviderPassword.IsNullOrWhiteSpace()) return null;
        try
        {
            var gameWallet = await gameProvider
                .GetWalletAsync(targetUser.UserExtend.GameProviderAccount);
            if (gameWallet.IsSuccess == false)
                throw new ExternalServiceException();

            return gameWallet.Data.Quota;
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to refresh G598 wallet, ex: {ex}", ex);
            return null;
        }
    }

    private async Task<decimal?> GetBaccaratWalletAsync(User targetUser)
    {
        if (targetUser.UserExtend is null
            || targetUser.UserExtend.GameBaccaratUserId.IsNullOrWhiteSpace()) return null;

        try
        {
            var externalWallet = await gameBaccaratService
                .GetWalletAsync(targetUser.UserExtend.GameBaccaratUserId);
            return externalWallet.Amount;
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to get external wallet", ex.Message);
            return null;
        }
    }

    private async Task<decimal?> GetElt998WalletAsync(User targetUser)
    {
        if (targetUser.UserExtend is null
            || targetUser.UserExtend.Etl998ProviderAccount.IsNullOrWhiteSpace()
            || targetUser.UserExtend.Etl998ProviderPassword.IsNullOrWhiteSpace()) return null;

        try
        {
            var request = new Etl998WalletRequest
            {
                Account = targetUser.UserExtend.Etl998ProviderAccount,
                Password = targetUser.UserExtend.Etl998ProviderPassword
            };

            var response = await etl998Service.GetWalletAsync(request);
            var externalWallet = response.FirstOrDefault();
            return externalWallet?.Money;
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to get external wallet", ex.Message);
            return null;
        }
    }

    private async Task<decimal?> GetSasSlotWalletAsync(User targetUser)
    {
        if (targetUser.UserExtend is null
            || targetUser.UserExtend.SasSlotAccount.IsNullOrWhiteSpace()) return null;

        try
        {
            var response = await sasSlotService.GetWalletAsync(targetUser.UserExtend.SasSlotAccount);
            return response;
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to get external wallet", ex.Message);
            return null;
        }
    }

    private async Task<decimal?> GetAtgWalletAsync(User targetUser)
    {
        var username = targetUser.UserExtend?.AtgUserName;
        if (username == null) return null;

        try
        {
            var response = await atgService.GetGameBalanceAsync(username);
            decimal.TryParse(response.Balance, out var balance);
            return balance;
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to get external wallet", ex.Message);
            return null;
        }
    }
    
    private async Task CreateOrUpdateExternalWalletAsync(
        string userId,
        UserWalletDto wallet,
        UserWalletExternalItemDto? targetWallet,
        GamePlatformDto platform,
        decimal? balance)
    {
        // Not create if null
        if (!balance.HasValue)
        {
            RemoveExternalWallet(wallet, platform.Id);

            var @event = new OnExternalApiFailedEvent(userId, platform.Id, ExternalApiAction.SyncWallet, string.Empty);
            await eventDispatcher.Publish(@event);
            return;
        }

        // Update target external wallet if exists
        if (targetWallet != null)
        {
            targetWallet.Amount = balance.Value;
            return;
        }

        // create new external wallet if platform wallet does not exist
        targetWallet = new UserWalletExternalItemDto
        {
            PlatformId = platform.Id,
            PlatformName = platform.Name,
            Amount = balance.Value,
        };
        wallet.ExternalWallets.Add(targetWallet);
    }

    private static void RemoveExternalWallet(UserWalletDto wallet, Guid platformId)
    {
        wallet.ExternalWallets = [.. wallet.ExternalWallets.Where(w => w.PlatformId != platformId)];
    }
}