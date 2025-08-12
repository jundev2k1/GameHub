using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.ExternalApi.GameProvider.Dtos.Deposit;
using Microsoft.Extensions.Configuration;

namespace game_x.application.Features.Games.Commands.GameWallet.Deposit;

public sealed class WalletDepositHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IUserBalanceRepo userBalanceRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    IConfiguration configuration,
    IGameTransactionRepo gameTransactionRepo,
    Utils.GameTransactionSnoGenerator snoGenerator,
    IUnitOfWork unitOfWork,
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

        // Check balance first
        const NetworkType network = NetworkType.Tron;
        const string symbol = CryptoTokenSymbol.Usdt;

        var token = await cryptoTokenRepo.GetBySymbolAndNetworkAsync(symbol, network, ct)
            ?? throw new BadRequestException(MessageCode.Crypto.CryptoTokenNotFound);

        var userBalance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, token.Id, ct);
        if (userBalance == null)
            throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);

        if (userBalance.Amount < request.Quota)
            throw new BadRequestException(MessageCode.Accounting.InsufficientBalance);

        var sno = await snoGenerator.GenerateAsync("DP", ct);

        var gameProviderUrl = configuration.GetValue<string>("GameProviderSettings:Host")
            ?? throw new InvalidOperationException("Host is not configured.");

        // Create pending transaction first
        var gameTransaction = GameTransaction.Create(
            userId,
            sno,
            request.Quota,
            gameProviderUrl,
            GameTransactionType.Deposit,
            GameTransactionStatus.Pending
        );
        await gameTransactionRepo.AddAsync(gameTransaction, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var depositRequest = new DepositRequest
        {
            Account = targetUser.UserExtend.GameProviderAccount,
            Quota = request.Quota,
            Sno = sno
        };

        var result = await gameProvider.WalletDepositAsync(depositRequest, request.IpAddress!);

        // Update transaction status based on result
        gameTransaction.UpdateStatus(result.issuccess ? GameTransactionStatus.Completed : GameTransactionStatus.Failed);
        await gameTransactionRepo.UpdateAsync(gameTransaction, ct);

        if (result.issuccess)
        {
            userBalance.Amount -= request.Quota;
            await userBalanceRepo.PutUpdateAsync(userBalance, ct);
        }

        return result;
    }


}
