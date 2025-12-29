using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;
using game_x.application.Events.WalletBalanceAdjustmentRequested;
using game_x.application.Features.Games.Dtos;
using game_x.application.Features.Games.Services;
using game_x.application.Utils;
using game_x.share.ExternalApi.Etl998.Constants;
using game_x.share.ExternalApi.Etl998.Dtos.PrepareTransfer;
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
    IEtl998Service etl998Service,
    IGamePlatformService gamePlatformService,
    IGameProviderCacheService gameProviderCache,
    IWalletManagerCacheService walletManagerCache) : ICommandHandler<WalletDepositCommand, ListTransactionExternalDto>
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

        // Write a balance adjustment transaction if target-platform balance changed
        var balanceAdjustmentEvent = new WalletBalanceAdjustmentRequestedEvent(currentUser.Id, targetPlatform.Id);
        await eventDispatcher.Publish(balanceAdjustmentEvent, ct);

        // Create transaction
        var currentBalance = await GetUserBalanceAsync(currentUser.Id, request, ct);
        var serialNumber = GameProviderUtils.SnoGenerate();
        var txSourceType = GetTxSourceType(targetPlatform.Id);
        var transaction = CreateTransaction(
            currentUser.Id,
            serialNumber,
            currentBalance.CryptoToken.Id,
            request.Amount,
            targetPlatform.LocalId,
            txSourceType,
            request.Note);

        // Create transaction and adjust balance
        decimal? balanceAfter = null;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await userBalanceRepo.UpdateAsync(currentBalance.PublicId, balance =>
            {
                balance.AdjustAmount(request.Amount, false);
                balanceAfter = balance.Amount;
            }, ct);
            await unitOfWork.SaveChangesAsync(ct);

            // Rollback all processing if the transaction fails at the third party
            if (request.PlatformId == GameConstants.PLATFORM_ID_G598)
                await DepositToProviderWalletAsync(
                    gameProviderAccount: currentUser.UserExtend!.GameProviderAccount,
                    sno: serialNumber,
                    amount: request.Amount);

            if (request.PlatformId == GameConstants.PLATFORM_ID_GAMEBACCARAT)
                await DepositToBaccaratWalletAsync(
                    gameUserId: currentUser.UserExtend!.GameBaccaratUserId,
                    sno: serialNumber,
                    amount: request.Amount);

            if (request.PlatformId == GameConstants.PLATFORM_ID_ETL998_GAMEBACCARAT)
                await DepositToEtl998WalletAsync(
                    accountName: currentUser.UserExtend!.Etl998ProviderAccount,
                    password: currentUser.UserExtend!.Etl998ProviderPassword,
                    sno: serialNumber,
                    amount: request.Amount);

            var @event = new OnUserBalanceUpdatedEvent(transaction.UserId, targetPlatform.Id);
            await eventDispatcher.Publish(@event, ct);

            // Set balance after for transaction
            var walletRefreshed = await walletManagerCache.GetExternalWalletAsync(
                currentUser.Id,
                request.PlatformId);
            transaction.ConfirmGameTx(balanceAfter!.Value, walletRefreshed.Amount);
            await transactionRepo.AddAsync(transaction, ct);
        }, ct);

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

    private static Transaction CreateTransaction(
        string userId,
        string serialNumber,
        int cryptoTokenId,
        decimal amount,
        int localPlatformId,
        TransactionSourceType sourceType,
        string? note)
    {
        var tx = Transaction.Create(
            userId: userId,
            amount: -amount,
            gameAmount: amount,
            cryptoTokenId: cryptoTokenId,
            sourceType: sourceType,
            type: TransactionType.Deposit,
            note: note);

        var txExternal = TransactionExternal.Create(
            sno: serialNumber,
            gamePlatformId: localPlatformId);
        tx.AddTxExternal(txExternal);

        return tx;
    }

    private static TransactionSourceType GetTxSourceType(Guid platformId)
    {
        if (platformId == GameConstants.PLATFORM_ID_G598)
            return TransactionSourceType.G598SnoGameProvider;
        if (platformId == GameConstants.PLATFORM_ID_GAMEBACCARAT)
            return TransactionSourceType.BaccaratGameProvider;
        if (platformId == GameConstants.PLATFORM_ID_ETL998_GAMEBACCARAT)
            return TransactionSourceType.Elt998GameProvider;

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

    private async Task DepositToBaccaratWalletAsync(string gameUserId, string sno, decimal amount)
    {
        var request = new GameBaccaratDepositRequest
        {
            UserId = gameUserId,
            Amount = amount,
            Sno = sno,
        };
        await gameBaccarat.DepositAsync(request);
    }

    private async Task DepositToEtl998WalletAsync(
        string accountName,
        string password,
        decimal amount,
        string sno)
    {
        var prepareRequest = new Etl998TransferRequest
        {
            Account = accountName,
            Password = password,
            CustomerOrderId = sno,
            Credit = amount,
            Type = Etl998CreditType.Deposit
        };
        await etl998Service.PrepareTransferAsync(prepareRequest);
        await etl998Service.ConfirmTransferAsync(prepareRequest);
    }
}