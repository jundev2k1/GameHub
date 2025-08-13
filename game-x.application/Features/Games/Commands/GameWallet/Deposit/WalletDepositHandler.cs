using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Utils;
using game_x.share.ExternalApi.GameProvider.Dtos.Deposit;
using MediatR;

namespace game_x.application.Features.Games.Commands.GameWallet.Deposit;

public sealed class WalletDepositHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IUserBalanceRepo userBalanceRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    IGameTransactionRepo gameTransactionRepo,
    IUnitOfWork unitOfWork,
    IGameProviderService gameProvider) : IRequestHandler<WalletDepositCommand>
{
    public async Task Handle(WalletDepositCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var targetUser = await userRepo.GetUserByIdAsync(userId, ct);
        if (targetUser.UserExtend is null)
            throw new NotFoundException("User extend is not exists.");

        if (!targetUser.EmailConfirmed)
            throw new BadRequestException(MessageCode.User.UserNotConfirmed);

        // Check balance first
        var token = await cryptoTokenRepo.GetBySymbolAndNetworkAsync(CryptoTokenSymbol.Usdt, NetworkType.Tron, ct)
            ?? throw new BadRequestException(MessageCode.Crypto.CryptoTokenNotFound);

        var userBalance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, token.Id, ct);
        if (userBalance == null)
            throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);

        if (userBalance.Amount < request.Amount)
            throw new BadRequestException(MessageCode.Accounting.InsufficientBalance);

        var sno = GameProviderUtils.SnoGenerate();

        var depositRequest = new GameDepositRequest
        {
            Account = targetUser.UserExtend.GameProviderAccount,
            Quota = request.Amount,
            Sno = sno
        };

        var result = await gameProvider.DepositWalletAsync(depositRequest);

        if (!result.IsSuccess)
            throw new BadRequestException(MessageCode.Accounting.CannotDepositToSystemWallet);

        try
        {
            var gameTransaction = GameTransaction.Create(
                userId,
                sno,
                request.Amount,
                GamePlatform.G598,
                GameTransactionType.Deposit
            );

            await gameTransactionRepo.AddAsync(gameTransaction, ct);
            userBalance.Amount -= request.Amount;
            await userBalanceRepo.PutUpdateAsync(userBalance, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create local transaction. Game provider deposit may need manual rollback. SNO: {sno}", ex);
        }
    }
}
