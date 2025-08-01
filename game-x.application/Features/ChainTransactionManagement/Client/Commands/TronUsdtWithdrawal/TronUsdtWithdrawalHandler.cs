using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Infrastructure.Services.Wallet;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.ChainTransactionManagement.Mapping;
using game_x.application.Utils;
using game_x.share.ExternalApi.Uxm.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.ChainTransactionManagement.Client.Commands.TronUsdtWithdrawal;

public sealed class TronUsdtWithdrawalHandler(
    IUxmService uxmService,
    IUserBalanceService userBalanceService,
    IUnitOfWork unitOfWork,
    IAsymmetricCryptoService asymmetricCryptoService,
    IUserAccessor userAccessor,
    IChainTransactionRepo chainTransactionRepo,
    ICryptoTokenRepo cryptoTokenRepo,
    IUserBalanceRepo userBalanceRepo,
    // IApplicationEventDispatcher eventDispatcher,
    IAppLogger<ChainTransaction> logger,
    IOptions<GameXSettings> galaxySettings,
    IAsymmetricKeyCacheService asymmetricKeyCacheService) : ICommandHandler<TronUsdtWithdrawalCommand, Unit>
{
    public async Task<Unit> Handle(TronUsdtWithdrawalCommand request, CancellationToken ct)
    {
        string userId = userAccessor.GetUserId();
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var (token, balance, feeAmount, totalAmount) = await ResolveBalanceInfoAsync(userId, request.Amount, ct);
            
            // Create ChainTransaction (not yet submitted)
            var chainTransaction = await CreateWithdrawalChainTransaction(request, userId, feeAmount, token.Id, ct);
            
            await FreezeBalanceAndCreateTxLogAsync(chainTransaction, balance, totalAmount, ct);
            
            await SendWithdrawalAsync(request, chainTransaction, balance, ct);
            
            return Unit.Value;
        }
        catch (BadRequestException)
        {
            await unitOfWork.RollbackAsync(ct);
            throw new BadRequestException("Invalid signature.");
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw new BadRequestException("Order creation failed.");
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
            transactionHash: "",
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
    
    private async Task FreezeBalanceAndCreateTxLogAsync(ChainTransaction chainTransaction, UserBalance balance, decimal totalAmount, CancellationToken ct)
    {
        await unitOfWork.BeginTransactionAsync(ct);

        try
        {
            await chainTransactionRepo.AddAsync(chainTransaction, ct);

            userBalanceService.Freeze(balance, totalAmount);
            await userBalanceRepo.PutUpdateAsync(balance.PublicId, balance, ct);

            await unitOfWork.CommitAsync(ct);
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }
    
    private async Task SendWithdrawalAsync(
        TronUsdtWithdrawalCommand request,
        ChainTransaction chainTransaction, 
        UserBalance balance, 
        CancellationToken ct)
    {     
        try
        {
            var gameXPrivateKey = asymmetricKeyCacheService.GameXPrivateKey;
            var merchantNumber = galaxySettings.Value.MerchantNumber;
        
            // Create UXM request data
            var requestData = request.ToUxmWithdrawalOrderRequestData(merchantNumber);
            var uxmRequest = new SecureRequest<UxmWithdrawalOrderRequest>
            {
                Data = requestData,
                Signature = asymmetricCryptoService.Sign(gameXPrivateKey, requestData)
            };
            
            await uxmService.CreateWithdrawalOrderAsync(uxmRequest);
        }
        catch (Exception ex)
        {
            await chainTransactionRepo.UpdateAsync(chainTransaction.PublicId, x =>
            {
                x.Status = ChainTransactionStatus.Failed;
                x.UpdateMeta(m => m.ErrorMessage = ex.Message);
            }, ct);

            await TryRefundFrozenBalanceAsync(chainTransaction, balance, ct);

            throw new BadRequestException(MessageCode.System.InvalidOperation, ex.Message);
        }
    }
    
    private async Task TryRefundFrozenBalanceAsync(ChainTransaction chainTransaction, UserBalance balance, CancellationToken ct)
    {
        const int maxRetries = 3;
        int attempt = 0;
        decimal refundAmount = chainTransaction.Amount + chainTransaction.Fee;

        while (attempt < maxRetries)
        {
            attempt++;

            try
            {
                await unitOfWork.BeginTransactionAsync(ct);

                userBalanceService.Unfreeze(balance, refundAmount);
                await userBalanceRepo.PutUpdateAsync(balance.PublicId, balance, ct);

                await unitOfWork.CommitAsync(ct);
                return;
            }
            catch (Exception ex)
            {
                await unitOfWork.RollbackAsync(ct);

                logger.LogError("[TronWithdrawal] ❌ 第 {Attempt} Balance compensation failed，UserId={UserId}, TokenId={TokenId}, OrderNo={OrderNo}, Refund={RefundAmount}, Err={ex}",
                    attempt, chainTransaction?.UserId, chainTransaction?.CryptoTokenId, chainTransaction?.OrderNumber, refundAmount, ex);

                await Task.Delay(200, ct); // Retry after a short delay
            }
        }

        logger.LogError(
            "[TronWithdrawal] ❌ Compensation failure exceeds the maximum number of retries，UserId={UserId}, TokenId={TokenId}, OrderNo={OrderNo}, Refund={RefundAmount}",
            chainTransaction?.UserId, chainTransaction?.CryptoTokenId, chainTransaction?.OrderNumber, refundAmount);
    }
}