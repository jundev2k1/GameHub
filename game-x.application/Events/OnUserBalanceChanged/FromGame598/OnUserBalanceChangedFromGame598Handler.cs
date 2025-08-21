using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.ExternalApi.GameProvider.Dtos.Wallet;

namespace game_x.application.Events.OnUserBalanceChanged.FromGame598;

public sealed class OnUserBalanceChangedHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IGameProviderService gameProviderService,
    IClientHubService clientHubService,
    IAppLogger<User> logger) : IApplicationEventHandler<OnUserBalanceChangedFromGame598Event>
{
    public async Task Handle(OnUserBalanceChangedFromGame598Event @event, CancellationToken ct = default)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await SendToMember(@event.UserId, ct);
        }, ct);
    }

    private async Task SendToMember(string userId, CancellationToken ct)
    {
        // Get user details with all balances
        var userDetail = await userRepo.GetUserDetailAsync(userId, ct);
        var gameBalance = await GetExternalWallet(userDetail.UserExtendInfo.GameProviderAccount);

        // Create site balances from user balances
        var siteBalances = userDetail.Balances.Select(b => b.Adapt<ClientCryptoBalanceDto>()).ToArray();

        var walletsData = new ClientWalletsDto(
            SiteBalances: siteBalances,
            GameBalance: gameBalance
        );

        await clientHubService.SendWalletsToMemberAsync(
            userId,
            walletsData);
    }

    private async Task<decimal?> GetExternalWallet(string account)
    {
        try
        {
            var externalRequest = new WalletRequest { Account = account };
            var externalWallet = await gameProviderService.GetWalletAsync(externalRequest);
            return externalWallet.Quota;
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to get external wallet", ex.Message);
            return null;
        }
    }
}