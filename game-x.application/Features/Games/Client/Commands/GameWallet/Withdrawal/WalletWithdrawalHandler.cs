using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Atg;
using game_x.application.Contract.Infrastructure.ExternalApi.GameBaccarat;
using game_x.application.Contract.Infrastructure.ExternalApi.GameProvider;
using game_x.application.Contract.Infrastructure.ExternalApi.IEtl998;
using game_x.application.Contract.Infrastructure.ExternalApi.SasSlot;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnUserBalanceUpdated;
using game_x.application.Events.WalletBalanceAdjustmentRequested;
using game_x.application.Features.Games.Dtos;
using game_x.application.Features.Games.Services;
using game_x.application.Utils;
using game_x.share.ExternalApi.Etl998.Constants;
using game_x.share.ExternalApi.Etl998.Dtos.PrepareTransfer;
using game_x.share.ExternalApi.GameBaccarat.Dtos.Withdrawal;
using game_x.share.ExternalApi.GameProvider.Dtos.Withdrawal;

namespace game_x.application.Features.Games.Client.Commands.GameWallet.Withdrawal;

public sealed class WalletWithdrawalHandler(
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
    ISasSlotService sasSlotService,
    IAtgService atgService,
    IGameProviderCacheService gameProviderCache,
    IGamePlatformService gamePlatformService,
    IWalletManagerCacheService walletManagerCache) : ICommandHandler<WalletWithdrawalCommand, ListTransactionExternalDto>
{
    public async Task<ListTransactionExternalDto> Handle(WalletWithdrawalCommand request, CancellationToken ct = default)
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

        // Get and check if a user's wallet has enough money?
        var wallet = await walletManagerCache.GetExternalWalletAsync(
            currentUser.Id,
            request.PlatformId);
        var actualAmount = request.PlatformId == GameConstants.PLATFORM_ID_SASSLOT
            ? wallet.Amount
            : request.Amount;
        if (wallet.Amount < actualAmount)
            throw new BadRequestException(MessageCode.Accounting.InsufficientBalance);

        // Create transaction
        var currentBalance = await GetUserBalanceAsync(currentUser.Id, request, ct);
        var serialNumber = GameProviderUtils.SnoGenerate();
        var transaction = CreateTransaction(
            currentUser.Id,
            serialNumber,
            currentBalance.CryptoToken.Id,
            actualAmount,
            targetPlatform.LocalId,
            request.Note);

        decimal? balanceAfter = null;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await userBalanceRepo.UpdateAsync(currentBalance.PublicId, balance =>
            {
                balance.AdjustAmount(actualAmount, true);
                balanceAfter = balance.TotalAmount;
            }, ct);

            await unitOfWork.SaveChangesAsync(ct);

            // Rollback all processing if the transaction fails at the third party
            if (request.PlatformId == GameConstants.PLATFORM_ID_G598)
                await WithdrawalToProviderWalletAsync(
                    gameProviderAccount: currentUser.UserExtend!.GameProviderAccount,
                    sno: transaction.TransactionExternal!.SerialNumber,
                    amount: actualAmount);

            if (request.PlatformId == GameConstants.PLATFORM_ID_GAMEBACCARAT)
                await WithdrawalToBaccaratWalletAsync(
                    gameUserId: currentUser.UserExtend!.GameBaccaratUserId,
                    sno: transaction.TransactionExternal!.SerialNumber,
                    amount: actualAmount);

            if (request.PlatformId == GameConstants.PLATFORM_ID_ETL998_GAMEBACCARAT)
                await WithdrawalToEtl998WalletAsync(
                    accountName: currentUser.UserExtend!.Etl998ProviderAccount,
                    password: currentUser.UserExtend!.Etl998ProviderPassword,
                    sno: serialNumber,
                    amount: actualAmount);

            if (request.PlatformId == GameConstants.PLATFORM_ID_SASSLOT)
                await WithdrawalToSasSlotWalletAsync(
                    account: currentUser.UserExtend!.SasSlotAccount,
                    sno: serialNumber,
                    amount: actualAmount);
            
            if (request.PlatformId == GameConstants.PLATFORM_ID_ATG)
                await WithdrawalToAtgWalletAsync(
                    account: currentUser.UserExtend!.AtgUserName,
                    sno: serialNumber,
                    amount: actualAmount);

            var @event = new OnUserBalanceUpdatedEvent(transaction.UserId, targetPlatform.Id);
            await eventDispatcher.Publish(@event, ct);

            // Set balance after for transaction
            var walletRefreshed = await walletManagerCache.GetExternalWalletAsync(
                currentUser.Id,
                request.PlatformId);
            transaction.ConfirmGameTx(actualAmount, balanceAfter!.Value, walletRefreshed.Amount);
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

    private async Task<UserBalance> GetUserBalanceAsync(string userId, WalletWithdrawalCommand command, CancellationToken ct)
    {
        var token = await cryptoTokenRepo.GetByIdAsync(command.CryptoTokenId, ct);
        if (token.Status != CryptoTokenStatus.Active)
            throw new BadRequestException(MessageCode.Crypto.CryptoTokenUnsupported);

        var userBalance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, token.Id, ct)
            ?? throw new BadRequestException(MessageCode.Accounting.BalanceNotFound);

        return userBalance;
    }

    private static Transaction CreateTransaction(
        string userId,
        string serialNumber,
        int cryptoTokenId,
        decimal amount,
        int localPlatformId,
        string? note)
    {
        var tx = Transaction.Create(
            type: TransactionType.Withdrawal,
            userId: userId,
            amount: amount,
            cryptoTokenId: cryptoTokenId,
            note: note);

        var txExternal = TransactionExternal.Create(
            sno: serialNumber,
            gamePlatformId: localPlatformId);
        tx.AddTxExternal(txExternal);

        return tx;
    }

    private async Task WithdrawalToProviderWalletAsync(string gameProviderAccount, string sno, decimal amount)
    {
        var withdrawalRequest = new GameWithdrawalRequest
        {
            Account = gameProviderAccount,
            Quota = amount,
            Sno = sno
        };

        var result = await gameProvider.WithdrawalWalletAsync(withdrawalRequest);
        if (!result.IsSuccess)
        {
            if (result.ErrorCode == GameMessage.InsufficientBalance)
                throw new BadRequestException(MessageCode.Accounting.InsufficientBalance);

            throw new BadRequestException(MessageCode.Accounting.WithdrawalToProviderWalletFailed);
        }
    }

    private async Task WithdrawalToBaccaratWalletAsync(string gameUserId, string sno, decimal amount)
    {
        var withdrawalRequest = new GameBaccaratWithdrawalRequest
        {
            UserId = gameUserId,
            Amount = amount,
            Sno = sno
        };

        await gameBaccarat.WithdrawalAsync(withdrawalRequest);
    }

    private async Task WithdrawalToEtl998WalletAsync(
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
            Type = Etl998CreditType.Withdrawal
        };
        await etl998Service.PrepareTransferAsync(prepareRequest);
        await etl998Service.ConfirmTransferAsync(prepareRequest);
    }

    private async Task WithdrawalToSasSlotWalletAsync(
        string account,
        decimal amount,
        string sno)
    {
        await sasSlotService.WithdrawalAsync(account, amount, sno);
    }
    
    private async Task WithdrawalToAtgWalletAsync(string account, decimal amount, string sno)
    {
        await atgService.UpdateGameBalanceAsync(
            username: account,
            balance: amount,
            action: "out",
            transferId: sno);
    }
}