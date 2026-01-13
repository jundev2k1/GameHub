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
    IGamePlatformBalanceRepo platformBalanceRepo,
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

    public async Task InitExternalWalletAsync(string userId, UserWalletDto userWalletDto, CancellationToken ct = default)
    {
        var balances = await platformBalanceRepo.GetBalancesByUserIdAsync(userId, ct);
        foreach (var balance in balances)
        {
            var targetPlatform = gameProviderCache.PlatformList
                .FirstOrDefault(pl => pl.LocalId == balance.PlatformId);
            if (targetPlatform is null) return;

            var (availableBalance, lockedBalance, _) = balance.GetBalance();
            await CreateOrUpdateExternalWalletAsync(
                userId,
                userWalletDto,
                null,
                targetPlatform,
                availableBalance,
                lockedBalance,
                balance.LastSyncedAt);
        }
    }

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

        // Refresh internal wallet with the actual balance from DB
        var userBalances = await userBalanceRepo.GetBalancesByUserIdAsync(userId);
        var userWallet = new UserWalletDto
        {
            InternalWallets = userBalances.Adapt<List<UserWalletInternalItemDto>>(),
            ExternalWallets = [],
        };

        // Refresh external wallet with snapshots of lastest balances
        await InitExternalWalletAsync(userId, userWallet);
        
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
                availableBalance: balance,
                lockedBalance: 0);      // G598 platform doesn't return locked balance
        }

        if (platformId == GameConstants.PLATFORM_ID_GAMEBACCARAT)
        {
            // TODO (Baccarat wallet): update locked balance after Baccarat platform began supporting it
            var balance = await GetBaccaratWalletAsync(targetUser);
            await CreateOrUpdateExternalWalletAsync(
                userId: userId,
                wallet: userWallet,
                targetWallet: targetWallet,
                platform: gameProviderCache.BaccaratPlatform,
                availableBalance: balance,
                lockedBalance: 0);      // Baccarat platform doesn't support locked balance now, it will be added in the future
        }

        if (platformId == GameConstants.PLATFORM_ID_ETL998_GAMEBACCARAT)
        {
            var balance = await GetElt998WalletAsync(targetUser);
            await CreateOrUpdateExternalWalletAsync(
                userId: userId,
                wallet: userWallet,
                targetWallet: targetWallet,
                platform: gameProviderCache.Etl998Platform,
                availableBalance: balance?.Balance,
                lockedBalance: balance?.LockedBalance);
        }

        if (platformId == GameConstants.PLATFORM_ID_SASSLOT)
        {
            var balance = await GetSasSlotWalletAsync(targetUser);
            await CreateOrUpdateExternalWalletAsync(
                userId: userId,
                wallet: userWallet,
                targetWallet: targetWallet,
                platform: gameProviderCache.SasSlotPlatform,
                availableBalance: balance,
                lockedBalance: 0);      // SAS Slot platform doesn't support locked balance now
        }

        if (platformId == GameConstants.PLATFORM_ID_ATG)
        {
            var balance = await GetAtgWalletAsync(targetUser);
            await CreateOrUpdateExternalWalletAsync(
                userId: userId,
                wallet: userWallet,
                targetWallet: targetWallet,
                platform: gameProviderCache.AtgPlatform,
                availableBalance: balance,
                lockedBalance: 0);      // ATG platform doesn't return locked balance
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

    private async Task<(decimal Balance, decimal LockedBalance)?> GetElt998WalletAsync(User targetUser)
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
            return externalWallet != null ? (externalWallet.Money, externalWallet.LockMoney) : null;
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
        if (username.IsNullOrEmpty() ) return null;

        try
        {
            var response = await atgService.GetGameBalanceAsync(username!);
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
        decimal? availableBalance,
        decimal? lockedBalance,
        DateTime? lastSyncAt = null)
    {
        // Not create if null
        if (!availableBalance.HasValue)
        {
            var @event = new OnExternalApiFailedEvent(userId, platform.Id, ExternalApiAction.SyncWallet, string.Empty);
            await eventDispatcher.Publish(@event);
            return;
        }

        // Sync balance in database
        await platformBalanceRepo.SyncOrCreateAsync(
            userId,
            platform.LocalId,
            availableBalance.Value,
            lockedBalance ?? 0);

        // Update target external wallet if exists
        if (targetWallet != null)
        {
            targetWallet.Amount = availableBalance.Value;
            targetWallet.LockedAmount = lockedBalance ?? 0;
            targetWallet.LastSyncAt = lastSyncAt ?? DateTime.UtcNow;
            return;
        }

        // create new external wallet if platform wallet does not exist
        targetWallet = new UserWalletExternalItemDto
        {
            PlatformId = platform.Id,
            PlatformName = platform.Name,
            Amount = availableBalance.Value,
            LockedAmount = lockedBalance ?? 0,
            LastSyncAt = lastSyncAt ?? DateTime.UtcNow,
        };
        wallet.ExternalWallets.Add(targetWallet);
    }
}
