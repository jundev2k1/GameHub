using game_x.application.Contract.Infrastructure.ExternalApi.PaymentGateway;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Features.Transactions.Dtos;
using game_x.application.Features.Transactions.Mapping;
using game_x.application.Utils;
using game_x.share.ExternalApi.PaymentGateway.Dtos;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.Transactions.Client.Commands.TraceV2.TronUsdtDepositV2;

public sealed class CreateDepositChainTransactionHandler(
    IPaymentGatewayService pgService,
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    ICryptoTokenRepo cryptoTokenRepo,
    ITransactionRepo transactionRepo,
    IAsymmetricCryptoService asymmetricCryptoService,
    IOptions<GameXSettings> gameXSettings
) : ICommandHandler<TronUsdtDepositV2Command, DepositChainTransactionResponseDto>
{
    public async Task<DepositChainTransactionResponseDto> Handle(TronUsdtDepositV2Command request, CancellationToken ct)
    {
        await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            var userId = userAccessor.GetUserId();
            var tx = await CreateTransaction(request, userId, ct);
            var pgRequest = CreatePaymentGatewayRequest(tx, (int)request.Provider);
            var result = await pgService.ProxyDepositAsync(pgRequest);

            var secretKey = gameXSettings.Value.PaymentGatewaySecretKey;
            bool isValid = asymmetricCryptoService.PaymentGatewayVerifySignature(secretKey, result.Data, result.Signature);
            if (!isValid) throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");

            tx.UpdateProviderResponse(balanceAfter: null, amount: result.Data.Amount, to: result.Data.WalletAddress);

            await unitOfWork.CommitAsync(ct);

            var updatedTransaction = await transactionRepo.GetInternalByIdAsync(tx.PublicId, ct);

            return new DepositChainTransactionResponseDto
            {
                Amount = result.Data.Amount,
                To = result.Data.WalletAddress,
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
        TronUsdtDepositV2Command request,
        string userId,
        CancellationToken ct)
    {
        var token = await cryptoTokenRepo.GetByIdAsync(request.CryptoTokenId, ct);
        if (token.Status != CryptoTokenStatus.Active)
            throw new BadRequestException(MessageCode.Crypto.CryptoTokenUnsupported);

        var orderNumber = OrderNoGenerator.Otc();
        var txInternal = TransactionInternal.Create(orderNumber: orderNumber, providerId: request.Provider);
        var tx = Transaction.Create(
            type: TransactionType.Deposit,
            userId: userId,
            amount: request.Amount,
            cryptoTokenId: token.Id,
            note: request.Note);
        tx.AddTxInternal(txInternal);
        await transactionRepo.AddAsync(tx, ct);
        return tx;
    }

    private SecureRequest<DepositOrderRequest> CreatePaymentGatewayRequest(Transaction tx, int providerId)
    {
        var secretKey = gameXSettings.Value.PaymentGatewaySecretKey;
        var payload = tx.ToPaymentGatewayDepositOrderRequest(providerId: providerId);
        var signature = asymmetricCryptoService.PaymentGatewaySign(secretKey, payload);
        return new SecureRequest<DepositOrderRequest> { Data = payload, Signature = signature };
    }
}
