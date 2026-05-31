using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Events.Transactions.OnConfirmTransaction;

namespace game_x.application.Features.Transactions.Webhooks.FastPay.Commands.FastPayWithdrawalSuccess;

public sealed class FastPayWithdrawalSuccessHandler(
    IAsymmetricCryptoService asymmetricCryptoService,
    IAsymmetricKeyCacheService asymmetricKeyCacheService,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<FastPayWithdrawalSuccessCommand>
{
    public async Task<Unit> Handle(FastPayWithdrawalSuccessCommand request, CancellationToken ct = default)
    {
        var (requestData, signature) = request;

        // Verify UXM signature
        var isValid = asymmetricCryptoService.VerifySignature(
            asymmetricKeyCacheService.FastPayPublicKey,
            requestData,
            signature);
        if (!isValid) throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");

        var @event = new OnConfirmTransactionEvent(
            ProviderOrderId: requestData.OrderUid,
            Hash: requestData.Hash,
            OrderNumber: requestData.OrderNumber,
            ActualAmount: requestData.ActualAmount,
            CreatedAt: requestData.CreatedAt,
            ConfirmedAt: requestData.ConfirmedAt,
            Remark: requestData.Remark);
        await eventDispatcher.Publish(@event, ct);

        return Unit.Value;
    }
}
