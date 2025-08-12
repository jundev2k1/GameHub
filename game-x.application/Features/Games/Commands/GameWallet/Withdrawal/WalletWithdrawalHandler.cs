using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.ExternalApi.GameProvider.Dtos.Withdrawal;
using Microsoft.Extensions.Configuration;

namespace game_x.application.Features.Games.Commands.GameWallet.Withdrawal;

public sealed class WalletWithdrawalHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IUserBalanceRepo userBalanceRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    IConfiguration configuration,
    IGameTransactionRepo gameTransactionRepo,
    Utils.GameTransactionSnoGenerator snoGenerator,
    IUnitOfWork unitOfWork,
    IGameProviderService gameProvider) : ICommandHandler<WalletWithdrawalCommand, WalletWithdrawalResponse>
{
    public async Task<WalletWithdrawalResponse> Handle(WalletWithdrawalCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var targetUser = await userRepo.GetUserByIdAsync(userId, ct);
        if (targetUser.UserExtend is null)
            throw new NotFoundException("User extend is not exists.");

        if (!targetUser.EmailConfirmed)
            throw new BadRequestException(MessageCode.User.UserNotConfirmed);

        if (request.Quota <= 0)
            throw new BadRequestException("Amount must be greater than zero.");

        var sno = await snoGenerator.GenerateAsync("WT", ct);

        var gameProviderUrl = configuration.GetValue<string>("GameProviderSettings:Host")
            ?? throw new InvalidOperationException("Host is not configured.");

        // Create pending transaction first
        var gameTransaction = GameTransaction.Create(
            userId,
            sno,
            request.Quota,
            gameProviderUrl,
            GameTransactionType.Withdrawal,
            GameTransactionStatus.Pending
        );
        await gameTransactionRepo.AddAsync(gameTransaction, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var withdrawalRequest = new WithdrawalRequest
        {
            Account = targetUser.UserExtend.GameProviderAccount,
            Quota = request.Quota,
            Sno = sno
        };

        try
        {
            var result = await gameProvider.WalletWithdrawalAsync(withdrawalRequest, request.IpAddress!);

            // Update transaction status based on result
            gameTransaction.UpdateStatus(result.issuccess ? GameTransactionStatus.Completed : GameTransactionStatus.Failed);
            await gameTransactionRepo.UpdateAsync(gameTransaction, ct);

            if (result.issuccess)
            {
                const NetworkType network = NetworkType.Tron;
                const string symbol = CryptoTokenSymbol.Usdt;

                var token = await cryptoTokenRepo.GetBySymbolAndNetworkAsync(symbol, network, ct)
                    ?? throw new BadRequestException(MessageCode.Crypto.CryptoTokenNotFound);

                var userBalance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, token.Id, ct);
                if (userBalance == null)
                    throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);

                userBalance.Amount += request.Quota;
                await userBalanceRepo.PutUpdateAsync(userBalance, ct);
            }

            await unitOfWork.SaveChangesAsync(ct);
            return result;
        }
        catch
        {
            // Mark transaction as failed if any error occurs
            gameTransaction.UpdateStatus(GameTransactionStatus.Failed);
            await gameTransactionRepo.UpdateAsync(gameTransaction, ct);
            await unitOfWork.SaveChangesAsync(ct);
            throw;
        }
    }


}

