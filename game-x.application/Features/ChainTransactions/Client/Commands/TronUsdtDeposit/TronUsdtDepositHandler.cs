using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.application.Features.ChainTransactions.Mapping;
using game_x.application.Utils;
using game_x.share.ExternalApi.Uxm.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtDeposit;

public sealed class CreateDepositChainTransactionHandler(
    IUxmService uxmService,
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    ICryptoTokenRepo cryptoTokenRepo,
    ITransactionRepo transactionRepo,
    IAsymmetricCryptoService asymmetricCryptoService,
    IAsymmetricKeyCacheService asymmetricKeyCacheService,
    IOptions<GameXSettings> gameXSettings
) : ICommandHandler<TronUsdtDepositCommand, DepositChainTransactionResponseDto>
{
    public async Task<DepositChainTransactionResponseDto> Handle(TronUsdtDepositCommand request, CancellationToken ct)
    {
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var userId = userAccessor.GetUserId();
            var tx = await CreateTransaction(request, userId, ct);

            var uxmRequest = CreateUxmRequest(tx);
            var result = await uxmService.CreateDepositOrderAsync(uxmRequest);

            // Verify UXM signature
            var uxmPublicKey = asymmetricKeyCacheService.UxmPublicKey;
            var isValid = asymmetricCryptoService.VerifySignature(uxmPublicKey, result.Data, result.Signature);
            if (!isValid) throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");
            
            tx.UpdateUxmResponse(
                amount: result.Data.Amount,
                orderUid: result.Data.OrderUid,
                to: result.Data.To);
            
            await unitOfWork.CommitAsync(ct);

            var updatedTransaction = await transactionRepo.GetInternalByIdAsync(tx.PublicId, ct);
            
            return new DepositChainTransactionResponseDto
            {
                Amount = result.Data.Amount,
                To = result.Data.To,
                Transaction = updatedTransaction.Adapt<ListTransactionInternalDto>(),
            };
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    private async Task<Transaction> CreateTransaction(
        TronUsdtDepositCommand request, 
        string userId,
        CancellationToken ct)
    {
        var token = await cryptoTokenRepo.GetByIdAsync(request.CryptoTokenId, ct);
        if(token.Status != CryptoTokenStatus.Active)
            throw new BadRequestException(MessageCode.Crypto.CryptoTokenUnsupported);
        
        var orderNumber = await OrderNoGenerator.GenerateUniqueOtcOrderNoAsync(transactionRepo, ct);
        var txInternal = TransactionInternal.Create(orderNumber: orderNumber);
        
        var tx = Transaction.Create(
            sourceType: TransactionSourceType.Uxm,
            type: TransactionType.Deposit,
            userId: userId,
            amount: request.Amount,
            cryptoTokenId: token.Id,
            note: request.Note);
        
        tx.AddTxInternal(txInternal);
        
        await transactionRepo.AddAsync(tx, ct);
        return tx;
    }

    private SecureRequest<UxmDepositOrderRequest> CreateUxmRequest(Transaction tx)
    {
        var merchantNumber = gameXSettings.Value.MerchantNumber;
        var gameXPrivateKey = asymmetricKeyCacheService.GameXPrivateKey;
        
        var requestData = tx.ToUxmDepositOrderRequest(merchantNumber);

        return new SecureRequest<UxmDepositOrderRequest>
        {
            Data = requestData,
            Signature = asymmetricCryptoService.Sign(gameXPrivateKey, requestData)
        };
    }
}