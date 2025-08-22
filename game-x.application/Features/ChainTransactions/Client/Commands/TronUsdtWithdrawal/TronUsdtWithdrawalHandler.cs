using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.Services.Wallet;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnTransactionCreated;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.application.Utils;

namespace game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtWithdrawal;

public sealed class TronUsdtWithdrawalHandler(
    IUserBalanceService userBalanceService,
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IUserAccessor userAccessor,
    IChainTransactionRepo chainTransactionRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    IUserBalanceRepo userBalanceRepo,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<TronUsdtWithdrawalCommand, ChainTransactionDto>
{
    public async Task<ChainTransactionDto> Handle(TronUsdtWithdrawalCommand request, CancellationToken ct)
    {
        string userId = userAccessor.GetUserId();
        int minimumAmount = 10;
        if(request.Amount < minimumAmount)
            throw new BadRequestException(MessageCode.Accounting.InvalidAmount);

        await ValidateKyc(userId, ct);
        
        var (token, balance, feeAmount, totalAmount) = await ResolveBalanceInfoAsync(userId, request.Amount, request.CryptoTokenId, ct);

        var transaction = await CreateWithdrawalChainTransaction(request, userId, feeAmount, token.Id, ct);
        
        await unitOfWork.WithTransactionAsync( async () =>
        {
            await chainTransactionRepo.AddAsync(transaction, ct);
            
            userBalanceService.Freeze(balance, totalAmount);
            await userBalanceRepo.PutUpdateAsync(balance, ct);

            var createdTransaction = await chainTransactionRepo.GetByIdAsync(transaction.PublicId, ct);
            
            await eventDispatcher.Publish(new OnTransactionCreatedEvent(createdTransaction), ct);
        }, ct);
        
        var updatedTransaction = await chainTransactionRepo.GetByIdAsync(transaction.PublicId, ct);
        
        return updatedTransaction.Adapt<ChainTransactionDto>();
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
    
    private async Task<ChainTransaction> CreateWithdrawalChainTransaction(
        TronUsdtWithdrawalCommand request, 
        string userId, 
        decimal feeAmount,
        int tokenId,
        CancellationToken ct = default)
    {
        var orderNumber = await OrderNoGenerator.GenerateUniqueOtcOrderNoAsync(chainTransactionRepo, ct);
        
        var chainTransaction = ChainTransaction.Create(
            type: ChainTransactionType.Withdrawal,
            userId: userId,
            orderNumber: orderNumber,
            status: ChainTransactionStatus.Pending,
            amount: request.Amount,
            fee: feeAmount,
            cryptoTokenId: tokenId,
            fromAddress: "",
            toAddress: request.To,
            note: request.Note);

        return chainTransaction;
    }
    
    private async Task<(CryptoToken Token, UserBalance Balance, decimal FeeAmount, decimal TotalAmount)>
        ResolveBalanceInfoAsync(string userId, decimal amount, Guid cryptoTokenId, CancellationToken ct)
    {
        var token = await cryptoTokenRepo.GetByIdAsync(cryptoTokenId, ct);

        if (token.Status != CryptoTokenStatus.Active)
            throw new BadRequestException(MessageCode.Crypto.CryptoTokenUnsupported);
                
        var balance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, token.Id, ct)
            ?? throw new BadRequestException(MessageCode.Accounting.WalletNotFound);

        decimal feeAmount = 3m;
        decimal totalAmount = amount + feeAmount;

        if (balance.Amount < totalAmount)
            throw new BadRequestException(MessageCode.Accounting.InsufficientBalance);

        return (token, balance, feeAmount, totalAmount);
    }
}