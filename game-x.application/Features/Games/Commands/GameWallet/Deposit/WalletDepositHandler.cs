using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.ExternalApi.GameProvider.Dtos.Deposit;

namespace game_x.application.Features.Games.Commands.GameWallet.Deposit;

public sealed class WalletDepositHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IUserBalanceRepo userBalanceRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    IGameProviderService gameProvider) : ICommandHandler<WalletDepositCommand, WalletDepositResponse>
{
    public async Task<WalletDepositResponse> Handle(WalletDepositCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var targetUser = await userRepo.GetUserByIdAsync(userId, ct);
        if (targetUser.UserExtend is null)
            throw new NotFoundException("User extend is not exists.");

        if (!targetUser.EmailConfirmed)
            throw new BadRequestException(MessageCode.User.UserNotConfirmed);
        var depositRequest = new DepositRequest
        {
            Account = targetUser.UserExtend.GameProviderAccount,
            Quota = request.Quota,
            Sno = GenerateTransactionId()
        };

        var result = await gameProvider.WalletDepositAsync(depositRequest, request.IpAddress!);

        if (result.issuccess)
        {
            const NetworkType network = NetworkType.Tron;
            const string symbol = CryptoTokenSymbol.Usdt;
            
            var token = await cryptoTokenRepo.GetBySymbolAndNetworkAsync(symbol, network, ct)
                ?? throw new BadRequestException(MessageCode.Crypto.CryptoTokenNotFound);
                
            var userBalance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, token.Id, ct);
            if (userBalance != null)
            {
                userBalance.Amount -= request.Quota;
                await userBalanceRepo.PutUpdateAsync(userBalance, ct);
            }
        }

        return result;
    }

    private static string GenerateTransactionId()
    {
        return $"{DateTime.UtcNow:yyyyMMddHHmmss}{Random.Shared.Next(100000, 999999)}";
    }
}

