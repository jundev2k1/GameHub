using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Services.UserUsdtLedger;
using game_x.application.Contract.Infrastructure.Services.Wallet;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Utils;

namespace game_x.application.Features.ChainTransactions.Client.Commands.OnChainTransferConfirm;

public sealed class OnChainTransferConfirmedHandler(
    IUserUsdtLedgerService userUsdtLedgerService,
    IUserBalanceService userBalanceService,
    IAppLogger<OnChainTransferConfirmedHandler> logger,
    IUserBalanceRepo userBalanceRepo,
    IChainTransactionRepo chainTransactionRepo,
    IUnitOfWork unitOfWork
) : IRequestHandler<OnChainTransferConfirmedCommand, Unit>
{
    public async Task<Unit> Handle(OnChainTransferConfirmedCommand request, CancellationToken ct)
    {
        // var txAlreadyProcessed = await chainTransactionRepoRepo.GetByIdAsync(request.PublicId, ct);
        // if (txAlreadyProcessed == null)
        // {
        //     logger.LogWarning("Skip duplicate transaction Hash: {TxHash}, UserId: {UserId}, TokenId: {TokenId}", request.TxHash, request.UserId, request.CryptoTokenId);
        //     return Unit.Value;
        // }

        await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            var balance = await GetBalanceAsync(request.UserId, request.CryptoTokenId, ct);

            var chainTransaction = request.Type switch
            {
                ChainTransactionType.Deposit => await HandleDepositAsync(balance, request, ct),
                ChainTransactionType.Withdrawal => await HandleWithdrawAsync(balance, request, ct),
                _ => throw new BadRequestException(MessageCode.Transaction.InvalidTradeType),
            };

            await userBalanceRepo.PutUpdateAsync(balance, ct);

            if (chainTransaction != null)
            {
                await userUsdtLedgerService.CreateForChainTransactionAsync(chainTransaction);
            }

            await unitOfWork.CommitAsync(ct);

            // var result = await queryService.GetUserWalletsAsync(balance.UserId, ct);
            // await clientHubService.PushBalanceUpdateAsync(request.UserId, result);
            
            return Unit.Value;
        }
        catch (BadRequestException ex)
        {
            await unitOfWork.RollbackAsync(ct);
            logger.LogError("Balance adjustment failed: {Message}", ex);
            throw;
        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackAsync(ct);
            logger.LogError("Balance adjustment failed: {Message}", ex);
            throw new BadRequestException(MessageCode.System.SystemError);
        }
    }

    private async Task<UserBalance> GetBalanceAsync(string userId, int cryptoTokenId, CancellationToken ct)
    {
        var balance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, cryptoTokenId, ct);

        if (balance is null)
        {
            balance = new UserBalance
            {
                UserId = userId,
                CryptoTokenId = cryptoTokenId,
                Amount = 0
            };
            await userBalanceRepo.CreateAsync(balance);
        }

        return balance;
    }

    private async Task<ChainTransaction> HandleDepositAsync(UserBalance balance, OnChainTransferConfirmedCommand request, CancellationToken ct)
    {
        var orderNumber = await OrderNoGenerator.GenerateUniqueOtcOrderNoAsync(chainTransactionRepo, ct);
        balance.Amount += request.Amount;

        var chainTransaction = new ChainTransaction()
        {
            // TxHash = request.TxHash,
            FromAddress = request.FromAddress,
            OrderNumber = orderNumber,
            ToAddress = request.ToAddress,
            Amount = request.Amount,
            CryptoTokenId = request.CryptoTokenId,
            ConfirmedAt = DateTime.UtcNow,
            Type = request.Type,
            UserId = request.UserId,
            Status = request.Result ? ChainTransactionStatus.Approved : ChainTransactionStatus.Failed
        };

        await chainTransactionRepo.AddAsync(chainTransaction, ct);

        logger.LogInformation("Complete the deposit process，New Balance: {Balance}, Amount: {Amount}", balance.Amount, request.Amount);

        return chainTransaction;
    }

    private async Task<ChainTransaction?> HandleWithdrawAsync(UserBalance balance, OnChainTransferConfirmedCommand request, CancellationToken ct)
    {
        var transaction = await chainTransactionRepo.GetByOrderNumberAsync(request.OrderNumber, ct);
        if (transaction is null)
        {
            logger.LogWarning("Not found ChainTransaction UserId: {UserId}", request.UserId);
            return null;
        }

        var totalAmount = request.Amount + transaction.Fee;

        if (balance.FrozenAmount < totalAmount)
        {
            logger.LogWarning("The frozen amount is insufficient and the withdrawal cannot be completed，Frozen: {Frozen}, Required: {Required}, UserId: {UserId}", balance.FrozenAmount, totalAmount, request.UserId);
            throw new BadRequestException(MessageCode.Accounting.BalanceCorrupted,
                $"FrozenAmount: {balance.FrozenAmount}, Required: {totalAmount}, UserId: {request.UserId}");
        }

        if (request.Result)
        {
            transaction.Status = ChainTransactionStatus.Approved;
            userBalanceService.FinalizeFrozen(balance, totalAmount);
            logger.LogInformation("The withdrawal transaction is successful and the unfreezing is completed, Amount: {Total}, Balance: {Balance}", totalAmount, balance.Amount);
        }
        else
        {
            transaction.Status = ChainTransactionStatus.Failed;
            userBalanceService.Unfreeze(balance, totalAmount);
            logger.LogInformation("The withdrawal transaction failed and the frozen amount has been refunded, Amount: {Total}, Balance: {Balance}", totalAmount, balance.Amount);
        }

        transaction.ConfirmedAt = DateTime.UtcNow;
        // transaction.TxHash = request.TxHash;
        transaction.FromAddress = request.FromAddress;
        await chainTransactionRepo.PutUpdateAsync(transaction, ct);

        logger.LogInformation("ChainTransaction Update completed，UserId: {UserId}, TokenId: {TokenId}", request.UserId, request.CryptoTokenId);

        return transaction;
    }
}