using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.Services.EmailProcessor;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnTransactionInternalCreated;
using game_x.application.Features.Transactions.Dtos;
using game_x.application.Services.Verification;
using game_x.application.Utils;

namespace game_x.application.Features.Transactions.Client.Commands.TraceV1.TronUsdtWithdrawal;

public sealed class TronUsdtWithdrawalHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IUserAccessor userAccessor,
    ITransactionRepo transactionRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    IUserBalanceRepo userBalanceRepo,
    IEmailVerificationProcessor emailVerification,
    ISpamProtectionCacheService spamProtection,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<TronUsdtWithdrawalCommand, ListTransactionInternalDto>
{
    /// <summary>The minimum allowed amount for creating a withdrawal transaction</summary>
    private const decimal MinimumAmount = 10;
    /// <summary>The user ID from token</summary>
    private readonly string _userId = userAccessor.GetUserId();

    public async Task<ListTransactionInternalDto> Handle(TronUsdtWithdrawalCommand request, CancellationToken ct = default)
    {
        // Validate the data from input
        await ValidateRequestAsync(request, ct);

        // Create transaction entity
        var transaction = CreateTransaction(request);

        // Handle creating the transaction and freezing the balance
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await transactionRepo.AddAsync(transaction, ct);

            await userBalanceRepo.UpdateAsync(this.BalanceId, balance =>
            {
                balance.Freeze(this.TotalAmount);
            }, ct);
        }, ct);

        // Get the latest transaction after creating
        var createdTransaction = await transactionRepo.GetInternalByIdAsync(transaction.PublicId, ct);

        // Publish event
        var transactionDto = createdTransaction.Adapt<TransactionInternalDto>();
        await eventDispatcher.Publish(new OnTransactionInternalCreatedEvent(transactionDto), ct);

        return createdTransaction.Adapt<ListTransactionInternalDto>();
    }

    private async Task ValidateRequestAsync(TronUsdtWithdrawalCommand request, CancellationToken ct = default)
    {
        // 1. Check the minimum amount limit
        if (request.Amount < MinimumAmount)
            throw new BadRequestException(MessageCode.Accounting.InvalidAmount);

        // 2. Check if the user has been KYC verified
        var userKyc = await userRepo.GetKycProfileAsync(this._userId, ct);
        if (userKyc.Status != KycStatus.Approved)
            throw new BadRequestException(MessageCode.User.KycInvalid);

        var email = userKyc.User.Email!;

        // 3. Check if the token is valid?
        await ValidateTokenAsync(request.CryptoTokenId, request.To, ct);

        // 4. Check if the balance is sufficient
        await ValidateBalanceAsync(request.Amount, ct);

        // 5. Check if email is temporarily locked due to too many failed attempts
        var isLocked = await spamProtection.IsVerifyLockedAsync(email);
        if (isLocked)
        {
            var retryTime = await spamProtection.GetVerifyRetryAfterAsync(email);
            var retrySeconds = retryTime != null ? (int)retryTime.Value.TotalSeconds : 0;
            throw new BadRequestException(
                MessageCode.User.VerifyTooManyFailedAttempts,
                new { Cooldown = retrySeconds });
        }

        // 6. Validate the provided verification code
        var isValid = emailVerification.VerifyEmail(email, request.Code, VerificationPurposes.Withdrawal);
        if (!isValid)
        {
            // Increasing the failed verification count
            await spamProtection.RegisterVerifyFailureAsync(email);
            throw new BadRequestException(MessageCode.System.InvalidVerifyCode);
        }
        else
        {
            // Reset failed attempt counter on success
            await spamProtection.ResetVerifyAttemptAsync(email);
        }
    }

    private Transaction CreateTransaction(TronUsdtWithdrawalCommand request)
    {
        var orderNumber = OrderNoGenerator.Otc();
        var txInternal = TransactionInternal.Create(
            orderNumber: orderNumber,
            fromAddress: string.Empty,
            toAddress: request.To,
            sourceType: TransactionSourceType.Payment);

        var tx = Transaction.Create(
            type: TransactionType.Withdrawal,
            userId: this._userId,
            amount: request.Amount,
            fee: this.FeeAmount,
            cryptoTokenId: this.TokenId,
            note: request.Note);
        tx.AddTxInternal(txInternal);
        return tx;
    }

    private async Task ValidateTokenAsync(Guid cryptoTokenId, string to, CancellationToken ct)
    {
        var token = await cryptoTokenRepo.GetByIdAsync(cryptoTokenId, ct);
        if (token.Status != CryptoTokenStatus.Active)
            throw new BadRequestException(MessageCode.Crypto.CryptoTokenUnsupported);

        if (!TransactionInternal.IsValidAddress(token.Network, to))
            throw new BadRequestException(MessageCode.Transaction.InvalidTransactionAddress);

        this.TokenId = token.Id;
    }

    private async Task ValidateBalanceAsync(decimal amount, CancellationToken ct)
    {
        var balance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(this._userId, this.TokenId, ct)
            ?? throw new BadRequestException(MessageCode.Accounting.WalletNotFound);

        this.FeeAmount = UserBalance.GetWithdrawalFee();
        this.TotalAmount = amount + this.FeeAmount;

        if (balance.Amount < this.TotalAmount)
            throw new BadRequestException(MessageCode.Accounting.InsufficientBalance);

        this.BalanceId = balance.Id;
    }

    private int TokenId { get; set; }
    private int BalanceId { get; set; }
    private decimal FeeAmount { get; set; }
    private decimal TotalAmount { get; set; }
}
