using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Utils;
using game_x.share.ExternalApi.GameProvider.Dtos.Withdrawal;
using MediatR;

namespace game_x.application.Features.Games.Commands.GameWallet.Withdrawal;

public sealed class WalletWithdrawalHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IUserBalanceRepo userBalanceRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    IGameTransactionRepo gameTransactionRepo,
    IUnitOfWork unitOfWork,
    IGameProviderService gameProvider) : IRequestHandler<WalletWithdrawalCommand>
{
    public async Task Handle(WalletWithdrawalCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var targetUser = await userRepo.GetUserByIdAsync(userId, ct);
        if (targetUser.UserExtend is null)
            throw new NotFoundException("User extend is not exists.");

        if (!targetUser.EmailConfirmed)
            throw new BadRequestException(MessageCode.User.UserNotConfirmed);

        var sno = GameProviderUtils.SnoGenerate();

        var withdrawalRequest = new GameWithdrawalRequest
        {
            Account = targetUser.UserExtend.GameProviderAccount,
            Quota = request.Amount,
            Sno = sno
        };

        var result = await gameProvider.WithdrawalWalletAsync(withdrawalRequest);

        if (!result.IsSuccess)
            throw new BadRequestException(MessageCode.Accounting.CannotWithdrawToSystemWallet);

        try
        {
            var token = await cryptoTokenRepo.GetBySymbolAndNetworkAsync(CryptoTokenSymbol.Usdt, NetworkType.Tron, ct)
                ?? throw new BadRequestException(MessageCode.Crypto.CryptoTokenNotFound);

            var userBalance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, token.Id, ct);
            if (userBalance == null)
                throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);

            var gameTransaction = GameTransaction.Create(
                userId,
                sno,
                request.Amount,
                GamePlatform.G598,
                GameTransactionType.Withdrawal
            );

            await gameTransactionRepo.AddAsync(gameTransaction, ct);
            userBalance.Amount += request.Amount;
            await userBalanceRepo.PutUpdateAsync(userBalance, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create local transaction. Game provider withdrawal may need manual rollback. SNO: {sno}", ex);
        }
    }
}
