using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.Services.Wallet;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnTransactionCreated;
using game_x.application.Utils;

namespace game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtWithdrawal;

public sealed class TronUsdtWithdrawalHandler(
    IUserBalanceService userBalanceService,
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IChainTransactionRepo chainTransactionRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    IUserBalanceRepo userBalanceRepo,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<TronUsdtWithdrawalCommand, Unit>
{
    public async Task<Unit> Handle(TronUsdtWithdrawalCommand request, CancellationToken ct)
    {
        string userId = userAccessor.GetUserId();
        int minimumAmount = 10;
        if(request.Amount < minimumAmount)
            throw new BadRequestException(MessageCode.Accounting.InvalidAmount);
        var (token, balance, feeAmount, totalAmount) = await ResolveBalanceInfoAsync(userId, request.Amount, ct);

        var transaction = await CreateWithdrawalChainTransaction(request, userId, feeAmount, token.Id, ct);
            
        await FreezeBalanceAndCreateChainTransactionAsync(transaction, balance, totalAmount, ct);
        
        await eventDispatcher.Publish(new OnTransactionCreatedEvent(transaction), ct);
        
        return Unit.Value;
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
        ResolveBalanceInfoAsync(string userId, decimal amount, CancellationToken ct)
    {
        const NetworkType network = NetworkType.Tron;
        const string symbol = CryptoTokenSymbol.Usdt;
        
        var token = await cryptoTokenRepo.GetBySymbolAndNetworkAsync(symbol, network, ct)
                    ?? throw new BadRequestException(MessageCode.Crypto.CryptoTokenNotFound);

        var balance = await userBalanceRepo.GetByUserIdAndTokenIdAsync(userId, token.Id, ct)
                      ?? throw new BadRequestException(MessageCode.Accounting.WalletNotFound);

        decimal feeAmount = 3m;
        decimal totalAmount = amount + feeAmount;

        if (balance.Amount < totalAmount)
            throw new BadRequestException(MessageCode.Accounting.InsufficientBalance);

        return (token, balance, feeAmount, totalAmount);
    }
    
    private async Task FreezeBalanceAndCreateChainTransactionAsync(
        ChainTransaction chainTransaction, 
        UserBalance balance, 
        decimal totalAmount, 
        CancellationToken ct)
    {
        await unitOfWork.WithTransactionAsync( async () =>
        {
            await chainTransactionRepo.AddAsync(chainTransaction, ct);
            userBalanceService.Freeze(balance, totalAmount);
            await userBalanceRepo.PutUpdateAsync(balance, ct);
        }, ct);
    }
}