using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Accounts.User.Dtos;
using game_x.share.Extensions;
using game_x.share.ExternalApi.GameBaccarat.Dtos.GetWallet;
using game_x.share.ExternalApi.GameProvider.Dtos.Wallet;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class WalletManagerCacheService(
    IMemoryCache cache,
    IUserBalanceRepo userBalanceRepo,
    IUserRepo userRepo,
    IGameProviderCacheService gameProviderCache,
    IGameBaccaratService gameBaccaratService,
    IGameProviderService gameProvider,
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

    public async Task RefreshWalletAsync(string userId)
    {
        var cacheKey = $"{CacheKeyPrefix}{userId}";
        var userBalances = await userBalanceRepo.GetBalancesByUserIdAsync(userId);
        var userWallet = new UserWalletDto
        {
            InternalWallets = [.. userBalances.Select(ub => ub.Adapt<UserWalletInternalItemDto>())],
            ExternalWallets = [],
        };

        var g598Platform = gameProviderCache.G598Platform;
        var g598Balance = await GetGame598WalletAsync(userId);
        userWallet.ExternalWallets.Add(new UserWalletExternalItemDto
        {
            PlatformId = g598Platform.Id,
            PlatformName = g598Platform.Name,
            Amount = g598Balance ?? 0,
        });

        var baccaratPlatform = gameProviderCache.BaccaratPlatform;
        var baccaratBalace = await GetBaccaratWalletAsync(userId);
        userWallet.ExternalWallets.Add(new UserWalletExternalItemDto
        {
            PlatformId = baccaratPlatform.Id,
            PlatformName = baccaratPlatform.Name,
            Amount = baccaratBalace ?? 0,
        });

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

    public async Task RefreshExternalWalletAsync(string userId, Guid platformId, decimal balance)
    {
        var userWallet = await GetWalletAsync(userId);
        var targetWallet = userWallet.ExternalWallets
            .FirstOrDefault(w => w.PlatformId == platformId);
        if (targetWallet is null)
        {
            if (platformId == GameConstants.PLATFORM_ID_G598)
            {
                var g598Balance = await GetGame598WalletAsync(userId);
                if (g598Balance.HasValue)
                {
                    targetWallet = new UserWalletExternalItemDto
                    {
                        PlatformId = GameConstants.PLATFORM_ID_G598,
                        PlatformName = gameProviderCache.G598Platform.Name,
                        Amount = g598Balance.Value,
                    };
                    userWallet.ExternalWallets.Add(targetWallet);
                }
            }

            if (platformId == GameConstants.PLATFORM_ID_GAMEBACCARAT)
            {
                var baccaratBalance = await GetBaccaratWalletAsync(userId);
                if (baccaratBalance.HasValue)
                {
                    targetWallet = new UserWalletExternalItemDto
                    {
                        PlatformId = GameConstants.PLATFORM_ID_GAMEBACCARAT,
                        PlatformName = gameProviderCache.BaccaratPlatform.Name,
                        Amount = baccaratBalance.Value,
                    };
                    userWallet.ExternalWallets.Add(targetWallet);
                }
            }
        }
        else
        {
            targetWallet.Amount = balance;
        }

        var cacheKey = $"{CacheKeyPrefix}{userId}";
        Set(cacheKey, userWallet);
    }

    private async Task<decimal?> GetGame598WalletAsync(string userId)
    {
        var targetUser = await userRepo.GetUserByIdAsync(userId);
        if (targetUser.UserExtend is null
            || targetUser.UserExtend.GameProviderAccount.IsNullOrWhiteSpace()
            || targetUser.UserExtend.GameProviderPassword.IsNullOrWhiteSpace()) return null;

        var gameWallet = await gameProvider.GetWalletAsync(new GameWalletRequest
        {
            Account = targetUser.UserExtend.GameProviderAccount
        });
        if (gameWallet.IsSuccess == false)
            throw new ExternalServiceException();

        return gameWallet.Data.Quota;
    }

    private async Task<decimal?> GetBaccaratWalletAsync(string userId)
    {
        var targetUser = await userRepo.GetUserByIdAsync(userId);
        if (targetUser.UserExtend is null
            || targetUser.UserExtend.GameBaccaratUserId.IsNullOrWhiteSpace()) return null;

        try
        {
            if (userId is null) return null;

            var externalRequest = new GameBaccaratGetWalletRequest { UserId = userId };
            var externalWallet = await gameBaccaratService.GetWalletAsync(externalRequest);

            return externalWallet.Amount;
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to get external wallet", ex.Message);
            return null;
        }
    }
}
