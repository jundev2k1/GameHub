using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.ExternalApi.Uxm;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.ChainTransactions.Dtos;
using game_x.application.Utils;
using game_x.share.ExternalApi.Uxm.Dtos;
using Microsoft.Extensions.Configuration;

namespace game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtDeposit;

public sealed class CreateDepositChainTransactionHandler(
    IUxmService uxmService,
    IChainTransactionRepo chainTransactionRepo,
    IUnitOfWork unitOfWork,
    IAsymmetricCryptoService asymmetricCryptoService,
    IUserAccessor userAccessor,
    IConfiguration configuration,
    IAsymmetricKeyCacheService asymmetricKeyCacheService,
    ICryptoTokenRepo cryptoTokenRepo
) : ICommandHandler<TronUsdtDepositCommand, CreateChainTransactionResponseDto>
{
    public async Task<CreateChainTransactionResponseDto> Handle(TronUsdtDepositCommand request, CancellationToken ct)
    {
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var userId = userAccessor.GetUserId();

            // 1. Create local chain transaction
            var localTransaction = await CreateLocalChainTransaction(request, userId, ct);
            await unitOfWork.SaveChangesAsync(ct);

            // 2. Call UXM API
            var uxmRequest = CreateUxmRequest(request, localTransaction.PublicId, userId);
            var apiResponse = await uxmService.CreateDepositOrderAsync(uxmRequest);

            // 3. Verify signature from UXM
            ValidateUxmResponse(apiResponse);

            // 4. Update chain transaction
            await UpdateChainTransaction(localTransaction.PublicId, apiResponse, ct);

            // 5. Commit transaction
            await unitOfWork.CommitAsync(ct);

            return new CreateChainTransactionResponseDto
            {
                OrderUid = apiResponse.Data.OrderUid,
                Amount = apiResponse.Data.Amount,
                To = apiResponse.Data.To
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
            type: ChainTransactionType.Deposit,
            status: ChainTransactionStatus.Pending
        );

        await chainTransactionRepo.AddAsync(transaction, ct);
        return transaction;
    }

    private SecureRequest<UxmDepositOrderRequestData> CreateUxmRequest(
        TronUsdtDepositCommand request,
        Guid publicId,
        string userId)
    {
        var merchantNumber = configuration.GetValue<string>("GameXSettings:MerchantNumber")
            ?? throw new InvalidOperationException("MerchantNumber is not configured.");

        var requestData = new UxmDepositOrderRequestData(
            merchantNumber,
            request.Amount,
            publicId.ToString(),
            userId,
            request.Note
        );

        var signature = asymmetricCryptoService.Sign(
            asymmetricKeyCacheService.GameXPrivateKey,
            requestData
        );

        return new SecureRequest<UxmDepositOrderRequestData>
        {
            Data = requestData,
            Signature = signature
        };
    }

    private void ValidateUxmResponse(dynamic apiResponse)
    {
        var isValid = asymmetricCryptoService.VerifySignature(
            asymmetricKeyCacheService.UxmPublicKey,
            apiResponse.Data,
            apiResponse.Signature
        );

        if (!isValid)
            throw new BadRequestException("Invalid signature from UXM.");
    }

    private async Task UpdateChainTransaction(
        Guid publicId,
        dynamic result,
        CancellationToken ct)
    {
        await chainTransactionRepo.PatchUpdateAsync(
            publicId,
            tx =>
            {
                tx.Status = ChainTransactionStatus.Approved;
                tx.UpdatedAt = DateTime.UtcNow;
                tx.OrderUid = result.Data.OrderUid;
                tx.ToAddress = result.Data.To;
                tx.Amount = result.Data.Amount;
            },
            ct
        );
    }
}