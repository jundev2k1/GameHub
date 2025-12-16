using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Dtos;
using game_x.share.ExternalApi.GameBaccarat.Dtos.GetWallet;
using game_x.share.ExternalApi.GameProvider.Dtos.Wallet;

namespace game_x.application.Events.OnUserBalanceUpdated;

public sealed class OnUserBalanceUpdatedHandler(
    IUserRepo userRepo,
    IGameProviderService gameProviderService,
    IGameBaccaratService gameBaccaratService,
    IWalletManagerCacheService walletManagerCache,
    IClientHubService clientHubService,
    IAppLogger<User> logger) : IApplicationEventHandler<OnUserBalanceUpdatedEvent>
{
    public async Task Handle(OnUserBalanceUpdatedEvent @event, CancellationToken ct = default)
    {
        var userInfo = await userRepo.GetUserDetailAsync(@event.UserId, ct);
        await RefreshWalletCache(userInfo, @event.PlatformId);
        await SendToMember(@event.UserId);
    }

    private async Task SendToMember(string userId)
    {
        // Get user details with all balances
        var myWallet = await walletManagerCache.GetWalletAsync(userId);
        var walletsData = new ClientWalletsDto(
            InternalWallets: [.. myWallet.InternalWallets],
            ExternalWallets: [.. myWallet.ExternalWallets]);

        await clientHubService.SendWalletsToMemberAsync(
            userId,
            walletsData);
    }

    private async Task RefreshWalletCache(UserDetailDto userDetail, Guid? platformId)
    {
        // Refresh internal wallet
        foreach (var balance in userDetail.Balances)
        {
            await walletManagerCache.RefreshInternalWalletAsync(
                userDetail.UserId,
                balance.Id,
                balance.Amount,
                balance.FrozenAmount);
        }

        // If platformId is null, skip refreshing external wallet
        if (!platformId.HasValue) return;

        // Get external wallet from a third party
        var amount = platformId switch
        {
            var id when id == GameConstants.PLATFORM_ID_G598 =>
                await GetG598ExternalWallet(userDetail.UserExtendInfo!.GameProviderAccount),

            var id when id == GameConstants.PLATFORM_ID_GAMEBACCARAT =>
                await GetBaccaratExternalWallet(userDetail.UserExtendInfo!.GameBaccaratUserId),

            _ => null
        };
        if (amount is null)
            logger.LogWarning($"Failed to get external wallet for platform ({platformId}) and user ({userDetail.UserId})");

        await walletManagerCache.RefreshExternalWalletAsync(
            userDetail.UserId,
            platformId.Value,
            amount ?? 0);
    }

    private async Task<decimal?> GetG598ExternalWallet(string? account)
    {
        try
        {
            if (account is null) return null;

            var externalRequest = new GameWalletRequest { Account = account };
            var externalWallet = await gameProviderService.GetWalletAsync(externalRequest);

            return externalWallet.Data.Quota;
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to get external wallet", ex.Message);
            return null;
        }
    }

    private async Task<decimal?> GetBaccaratExternalWallet(string? userId)
    {
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
