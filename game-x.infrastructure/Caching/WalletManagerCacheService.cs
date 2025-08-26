using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Exceptions;
using game_x.application.Features.Accounts.User.Dtos;
using game_x.share.ExternalApi.GameProvider.Dtos.Login;
using game_x.share.ExternalApi.GameProvider.Dtos.Wallet;
using Microsoft.Extensions.Caching.Memory;

namespace game_x.infrastructure.Caching;

public sealed class WalletManagerCacheService(
    IMemoryCache cache,
    IUserAccessor userAccessor,
    IUserBalanceRepo userBalanceRepo,
    IUserRepo userRepo,
    IGameProviderService gameProvider,
    IGameAesEncryptor aesEncryptor) : CacheService(cache), IWalletManagerCacheService
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

        var g598Balance = await GetGame598Wallet(userId);
        userWallet.ExternalWallets.Add(new UserWalletExternalItemDto
        {
            PlatformId = Guid.NewGuid(),
            PlatformName = "",
            Amount = g598Balance,
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
            .FirstOrDefault(w => w.PlatformId == platformId)
            ?? throw new BadRequestException();

        targetWallet.Amount = balance;
        var cacheKey = $"{CacheKeyPrefix}{userId}";
        Set(cacheKey, userWallet);
    }

    private async Task<decimal> GetGame598Wallet(string userId)
    {
        var targetUser = await userRepo.GetUserByIdAsync(userId);
        if (targetUser.UserExtend is null)
            throw new NotFoundException("User extend is not exists.");
        var loginState = await gameProvider.LoginAsync(
            new LoginRequest
            {
                Account = targetUser.UserExtend.GameProviderAccount,
                Passwd = aesEncryptor.Decrypt(targetUser.UserExtend.GameProviderPassword)
            },
            userAccessor.GetIpAddress());
        if (loginState.IsSuccess == false)
            throw new ExternalServiceException();

        var gameWallet = await gameProvider.GetWalletAsync(new WalletRequest
        {
            Account = targetUser.UserExtend.GameProviderAccount
        });
        if (gameWallet.IsSuccess == false)
            throw new ExternalServiceException();

        return gameWallet.Quota;
    }
}
