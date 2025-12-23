using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Accounts.Dtos;

namespace game_x.application.Events.OnUserBalanceUpdated;

public sealed class OnUserBalanceUpdatedHandler(
    IUserRepo userRepo,
    IWalletManagerCacheService walletManagerCache,
    IClientHubService clientHubService) : IApplicationEventHandler<OnUserBalanceUpdatedEvent>
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

        await walletManagerCache.RefreshExternalWalletAsync(
            userDetail.UserId,
            platformId.Value);
    }
}
