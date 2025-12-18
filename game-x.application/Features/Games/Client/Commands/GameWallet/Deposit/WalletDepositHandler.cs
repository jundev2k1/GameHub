using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;
using game_x.application.Events.WalletBalanceAdjustmentRequested;
using game_x.application.Features.Games.Dtos;
using game_x.application.Features.Games.Services;
using game_x.application.Utils;
using game_x.share.ExternalApi.GameBaccarat.Dtos.Deposit;
using game_x.share.ExternalApi.GameProvider.Dtos.Deposit;

namespace game_x.application.Features.Games.Client.Commands.GameWallet.Deposit;

public sealed class WalletDepositHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IUserBalanceRepo userBalanceRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    ITransactionRepo transactionRepo,
    IUnitOfWork unitOfWork,
    IApplicationEventDispatcher eventDispatcher,
    IGameProviderService gameProvider,
    IGameBaccaratService gameBaccarat,
    IGamePlatformService gamePlatformService,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<WalletDepositCommand, ListTransactionExternalDto>
{
    public async Task<ListTransactionExternalDto> Handle(WalletDepositCommand request, CancellationToken ct = default)
    {
        var targetPlatform = gameProviderCache.PlatformList.FirstOrDefault(gp => gp.Id == request.PlatformId)
            ?? throw new NotFoundException(MessageCode.Accounting.PlatformNotExist);

        var currentUser = await GetCurrentUserAsync(ct);
        currentUser = await gamePlatformService.EnsureExternalAccountCreatedAsync(
            currentUser,
            request.PlatformId,
            ct: ct);

        // Write a balance ajustment transaction if target platform balance changed
        var balanceAjustmentEvent = new WalletBalanceAdjustmentRequestedEvent(currentUser.Id, targetPlatform.Id);
        await eventDispatcher.Publish(balanceAjustmentEvent, ct);

        var currentBalance = await GetUserBalanceAsync(currentUser.Id, request, ct);

        // Create transaction and ajust balance
        Transaction? transaction = null;
        var txSourceType = GetTxSourceType(targetPlatform.Id);
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await userBalanceRepo.UpdateAsync(currentBalance.PublicId, balance =>
            {
                balance.AdjustAmount(request.Amount, true);
            }, ct);

            transaction = await CreateTransactionAsync(
                currentUser.Id,
                currentBalance.CryptoToken.Id,
                request.Amount,
                currentBalance.TotalAmount,
                targetPlatform.LocalId,
                txSourceType,
                request.Note,
                ct);

            // Rollback all processing if the transaction fails at the third party
            if (request.PlatformId == GameConstants.PLATFORM_ID_G598)
            {
                await DepositToProviderWalletAsync(
                    gameProviderAccount: currentUser.UserExtend!.GameProviderAccount,
                    sno: transaction.TransactionExternal!.SerialNumber,
                    amount: transaction.Amount);
            }
            if (request.PlatformId == GameConstants.PLATFORM_ID_GAMEBACCARAT)
            {
                await DepositToBacaratWalletAsync(
                    gameUserId: currentUser.UserExtend!.GameBaccaratUserId,
                    sno: transaction.TransactionExternal!.SerialNumber,
                    amount: transaction.Amount);
            }
        }, ct);

        var @event = new OnUserBalanceUpdatedEvent(transaction!.UserId, targetPlatform.Id);
        await eventDispatcher.Publish(@event, ct);

        var result = await transactionRepo.GetExternalByIdAsync(transaction.PublicId, ct);
        return result.Adapt<ListTransactionExternalDto>();
    }

    private async Task<User> GetCurrentUserAsync(CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();
        var currentUser = await userRepo.GetUserByIdAsync(userId, ct);
        if (currentUser.UserExtend is null)
            throw new NotFoundException(MessageCode.User.UserExtendNotFound);

        if (!currentUser.EmailConfirmed)
            throw new BadRequestException(MessageCode.User.UserNotConfirmed);

        return currentUser;
    }

    private async Task<UserBalance> GetUserBalanceAsync(string userId, WalletDepositCommand command, CancellationToken ct)
    {
        var token = await cryptoTokenRepo.GetByIdAsync(command.CryptoTokenId, ct);
        if (token.Status != CryptoTokenStatus.Active)
            throw new BadRequestException(MessageCode.Crypto.CryptoTokenUnsupported);

        var userBalance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, token.Id, ct)
            ?? throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);

        if (userBalance.Amount < command.Amount)
            throw new BadRequestException(MessageCode.Accounting.InsufficientBalance);

        return userBalance;
    }

    private async Task<Transaction> CreateTransactionAsync(
        string userId,
        int cryptoTokenId,
        decimal amount,
        decimal balanceAfter,
        int localPlatformId,
        TransactionSourceType sourceType,
        string? note,
        CancellationToken ct)
    {
        var tx = Transaction.Create(
            sourceType: sourceType,
            type: TransactionType.Deposit,
            userId: userId,
            amount: amount,
            cryptoTokenId: cryptoTokenId,
            note: note);
        tx.ConfirmTx(balanceAfter, balanceAfter, DateTime.UtcNow);

        var txExternal = TransactionExternal.Create(
            sno: GameProviderUtils.SnoGenerate(),
            gamePlatformId: localPlatformId);
        tx.AddTxExternal(txExternal);
        await transactionRepo.AddAsync(tx, ct);

        return tx;
    }

    private static TransactionSourceType GetTxSourceType(Guid platformId)
    {
        if (platformId == GameConstants.PLATFORM_ID_G598)
            return TransactionSourceType.G598SnoGameProvider;
        if (platformId == GameConstants.PLATFORM_ID_GAMEBACCARAT)
            return TransactionSourceType.BaccaratGameProvider;

        throw new NotSupportedException($"This platform ({platformId}) is not support.");
    }

    private async Task DepositToProviderWalletAsync(string gameProviderAccount, string sno, decimal amount)
    {
        var depositRequest = new GameDepositRequest
        {
            Account = gameProviderAccount,
            Quota = amount,
            Sno = sno
        };

        var result = await gameProvider.DepositWalletAsync(depositRequest);
        if (!result.IsSuccess)
            throw new BadRequestException(MessageCode.Accounting.DepositToProviderWalletFailed);
    }

    private async Task DepositToBacaratWalletAsync(string gameUserId, string sno, decimal amount)
    {
        var request = new GameBaccaratDepositRequest
        {
            UserId = gameUserId,
            Amount = amount,
            Sno = sno,
        };
        await gameBaccarat.DepositAsync(request);
    }
}
