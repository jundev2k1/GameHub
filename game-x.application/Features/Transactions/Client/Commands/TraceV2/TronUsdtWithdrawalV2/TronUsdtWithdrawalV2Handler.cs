using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.Transactions.OnTransactionInternalCreated;
using game_x.application.Features.Transactions.Dtos;
using game_x.application.Utils;

namespace game_x.application.Features.Transactions.Client.Commands.TraceV2.TronUsdtWithdrawalV2;

public sealed class TronUsdtWithdrawalV2Handler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IUserAccessor userAccessor,
    ITransactionRepo transactionRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    IUserBalanceRepo userBalanceRepo,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<TronUsdtWithdrawalV2Command, ListTransactionInternalDto>
{
    public async Task<ListTransactionInternalDto> Handle(TronUsdtWithdrawalV2Command request, CancellationToken ct)
    {
        string userId = userAccessor.GetUserId();
        int minimumAmount = 10;
        if(request.Amount < minimumAmount)
            throw new BadRequestException(MessageCode.Accounting.InvalidAmount);

        await ValidateKyc(userId, ct);
        
        var (token, balance, feeAmount, totalAmount) = await ResolveBalanceInfoAsync(
            userId: userId, 
            amount: request.Amount, 
            cryptoTokenId: request.CryptoTokenId,
            to: request.To,
            ct: ct);

        var tx = CreateTransaction(request, userId, feeAmount, token.Id);

        await unitOfWork.WithTransactionAsync( async () =>
        {
            await transactionRepo.AddAsync(tx, ct);
            
            balance.Freeze(totalAmount);
            await userBalanceRepo.PutUpdateAsync(balance, ct);
            await eventDispatcher.Publish(new OnTransactionInternalCreatedEvent(tx.Adapt<TransactionInternalDto>()), ct);
        }, ct);
        
        return tx.Adapt<ListTransactionInternalDto>();
    }
   
    private async Task ValidateKyc(
        string userId, 
        CancellationToken ct = default)
    {
        try
        {
            var userKyc = await userRepo.GetKycProfileAsync(userId, ct)
                ?? throw new Exception();
        
            if(userKyc.Status != KycStatus.Approved)
                throw new Exception();
        }
        catch
        {
            throw new BadRequestException(MessageCode.User.KycInvalid);
        }
    }
    
    private static Transaction CreateTransaction(
        TronUsdtWithdrawalV2Command request, 
        string userId, 
        decimal feeAmount,
        int tokenId)
    {
        var orderNumber = OrderNoGenerator.Otc();
        var txInternal = TransactionInternal.Create(
            orderNumber: orderNumber,
            fromAddress: string.Empty,
            toAddress: request.To,
            providerId: request.Provider,
            sourceType: TransactionSourceType.Payment);
        
        var tx = Transaction.Create(
            type: TransactionType.Withdrawal,
            userId: userId,
            amount: request.Amount,
            fee: feeAmount,
            cryptoTokenId: tokenId,
            note: request.Note);

        tx.AddTxInternal(txInternal);
        return tx;
    }
    
    private async Task<(CryptoToken Token, UserBalance Balance, decimal FeeAmount, decimal TotalAmount)>
        ResolveBalanceInfoAsync(string userId, decimal amount, Guid cryptoTokenId, string to, CancellationToken ct)
    {
        var token = await ValidateTokenAsync(cryptoTokenId, to, ct);
        
        var balance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, token.Id, ct)
            ?? throw new BadRequestException(MessageCode.Accounting.WalletNotFound);

        decimal feeAmount = UserBalance.GetWithdrawalFee();
        decimal totalAmount = amount + feeAmount;

        if (balance.Amount < totalAmount)
            throw new BadRequestException(MessageCode.Accounting.InsufficientBalance);

        return (token, balance, feeAmount, totalAmount);
    }
    
    private async Task<CryptoToken> ValidateTokenAsync(Guid cryptoTokenId, string to, CancellationToken ct)
    {
        var token = await cryptoTokenRepo.GetByIdAsync(cryptoTokenId, ct);

        if (token.Status != CryptoTokenStatus.Active)
            throw new BadRequestException(MessageCode.Crypto.CryptoTokenUnsupported);
                
        if (!TransactionInternal.IsValidAddress(token.Network, to))
            throw new BadRequestException(MessageCode.Transaction.InvalidTransactionAddress);

        return token;
    }
}