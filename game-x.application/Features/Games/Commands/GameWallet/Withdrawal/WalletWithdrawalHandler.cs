using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Utils;
using game_x.share.ExternalApi.GameProvider.Dtos.Withdrawal;

namespace game_x.application.Features.Games.Commands.GameWallet.Withdrawal;

public sealed class WalletWithdrawalHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IUserBalanceRepo userBalanceRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    IGameTransactionRepo gameTransactionRepo,
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

        var sno = new G598SnoGenerator().Generate();

        // Check if transaction already exists (idempotency)
        var snoExists = await gameTransactionRepo.SnoExistsAsync(sno, ct);
        if (snoExists)
            throw new BadRequestException("Transaction already processed.");

        var withdrawalRequest = new WithdrawalRequest
        {
            Account = targetUser.UserExtend.GameProviderAccount,
            Quota = request.Quota,
            Sno = sno
        };

        var result = await gameProvider.WalletWithdrawalAsync(withdrawalRequest, request.IpAddress!);

        if (result.issuccess)
        {
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
                    request.Quota,
                    GamePlatform.G598,
                    GameTransactionType.Withdrawal
                );

                await gameTransactionRepo.AddAsync(gameTransaction, ct);
                userBalance.Amount += request.Quota;
                await userBalanceRepo.PutUpdateAsync(userBalance, ct);
                await unitOfWork.SaveChangesAsync(ct);
            }
            catch (Exception)
            {
                throw;
            }
        }
        return result;
    }
}
