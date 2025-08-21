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
    IChainTransactionRepo chainTransactionRepo,
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
            var localTransaction = await CreateLocalChainTransaction(request, userId, ct);
            await unitOfWork.SaveChangesAsync(ct);

            var uxmRequest = CreateUxmRequest(localTransaction);
            var result = await uxmService.CreateDepositOrderAsync(uxmRequest);

            // Verify UXM signature
            var uxmPublicKey = asymmetricKeyCacheService.UxmPublicKey;
            var isValid = asymmetricCryptoService.VerifySignature(uxmPublicKey, result.Data, result.Signature);
            if (!isValid) throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");
            
            await UpdateChainTransaction(localTransaction.PublicId, result.Data, ct);

            await unitOfWork.CommitAsync(ct);

            return new DepositChainTransactionResponseDto
            {
                TransactionId = localTransaction.PublicId,
                Amount = result.Data.Amount,
                To = result.Data.To
            };
        }
        catch
        {
            await unitOfWork.RollbackAsync(ct);
            throw;
        }
    }

    private async Task<ChainTransaction> CreateLocalChainTransaction(
        TronUsdtDepositCommand request,
        string userId,
        CancellationToken ct)
    {
        var token = await cryptoTokenRepo.GetByIdAsync(request.CryptoTokenId, ct);
        if(token.Status != CryptoTokenStatus.Active)
            throw new BadRequestException(MessageCode.Crypto.CryptoTokenUnsupported);
        
        var orderNumber = await OrderNoGenerator.GenerateUniqueOtcOrderNoAsync(chainTransactionRepo, ct);

        var transaction = ChainTransaction.Create(
            userId: userId,
            orderNumber: orderNumber,
            cryptoTokenId: token.Id,
            amount: request.Amount,
            note: request.Note,
            type: ChainTransactionType.Deposit,
            status: ChainTransactionStatus.Pending
        );

        await chainTransactionRepo.AddAsync(transaction, ct);
        return transaction;
    }

    private SecureRequest<UxmDepositOrderRequest> CreateUxmRequest(ChainTransaction chainTransaction)
    {
        var merchantNumber = gameXSettings.Value.MerchantNumber;
        var gameXPrivateKey = asymmetricKeyCacheService.GameXPrivateKey;
        
        var requestData = chainTransaction.ToUxmDepositOrderRequest(merchantNumber);

        return new SecureRequest<UxmDepositOrderRequest>
        {
            Data = requestData,
            Signature = asymmetricCryptoService.Sign(gameXPrivateKey, requestData)
        };
    }

    private async Task UpdateChainTransaction(
        Guid publicId,
        UxmDepositOrderResponseData data,
        CancellationToken ct)
    {
        await chainTransactionRepo.PatchUpdateAsync(
            publicId,
            tx =>
            {
                tx.OrderUid = data.OrderUid;
                tx.ToAddress = data.To;
                tx.Amount = data.Amount;
            },
            ct
        );
    }
}