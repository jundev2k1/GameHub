using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.SignalR.Dtos;
using game_x.application.Contract.Infrastructure.SignalR.Services;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.ExternalApi.GameProvider.Dtos.Wallet;

namespace game_x.application.Events.OnUserBalanceChanged.FromUxm;

public sealed class OnUserBalanceChangedFromUxmHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IGameProviderService gameProviderService,
    IChainTransactionRepo chainTransactionRepo,
    IAppLogger<ChainTransaction> logger,
    IClientHubService clientHubService) : IApplicationEventHandler<OnUserBalanceChangedFromUxmEvent>
{
    public async Task Handle(OnUserBalanceChangedFromUxmEvent @event, CancellationToken ct = default)
    {
        try
        {
            ChainTransaction? transaction =
                await chainTransactionRepo.GetByOrderNumberAsync(@event.OrderNumber ?? string.Empty, ct);

            if (transaction == null)
                throw new NotFoundException(MessageCode.Transaction.TradeNotFound,
                    $"Transaction with order number '{@event.OrderNumber}' not found.");

            UserBalance? balance = transaction.User?.UserBalances.FirstOrDefault(b => b.CryptoTokenId == transaction.CryptoTokenId);
            if (balance == null)
                throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);

            await unitOfWork.WithTransactionAsync(async () =>
            {
                await SendToMember(balance, ct);
            }, ct);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
        }

    }

    private async Task SendToMember(UserBalance balance, CancellationToken ct)
    {
        // Get user details with all balances
        var userDetail = await userRepo.GetUserDetailAsync(balance.UserId, ct);
        var gameBalance = await GetExternalWallet(userDetail.UserExtendInfo.GameProviderAccount);

        // Create site balances from user balances
        var siteBalances = userDetail.Balances.Select(b => new ClientCryptoBalanceDto(
            Amount: b.Amount,
            FrozenAmount: b.FrozenAmount,
            TotalAmount: b.TotalAmount,
            Network: b.Network,
            Symbol: b.Symbol
        )).ToArray();

        var walletsData = new ClientWalletsDto(
            SiteBalances: siteBalances,
            GameBalance: gameBalance
        );

        await clientHubService.SendWalletsToMemberAsync(
            balance.UserId,
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
        catch
        {
            return null;
        }
    }
}