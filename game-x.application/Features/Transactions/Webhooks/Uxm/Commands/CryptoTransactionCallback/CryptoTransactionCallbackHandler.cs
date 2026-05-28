using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Events.Transactions.OnConfirmTransaction;

namespace game_x.application.Features.Transactions.Webhooks.Uxm.Commands.CryptoTransactionCallback;

public sealed class CryptoTransactionCallbackHandler(
    IAsymmetricCryptoService asymmetricCryptoService,
    IAsymmetricKeyCacheService asymmetricKeyCacheService,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<CryptoTransactionCallbackCommand>
{
    public async Task<Unit> Handle(CryptoTransactionCallbackCommand request, CancellationToken ct = default)
    {
        var (requestData, signature) = request;

        // Verify UXM signature
        // var uxmPublicKey = asymmetricKeyCacheService.UxmPublicKey;
        // var isValid = asymmetricCryptoService.VerifySignature(uxmPublicKey, requestData, signature);
        // if (!isValid) throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");

        await eventDispatcher.Publish(new OnConfirmTransactionEvent(
            ProviderOrderId: requestData.OrderUid,
            Hash: requestData.Hash,
            OrderNumber: requestData.OrderNumber,
            ActualAmount: requestData.ActualAmount,
            CreatedAt: requestData.CreatedAt,
            ConfirmedAt: requestData.ConfirmedAt,
            Remark: requestData.Remark), ct);

        return Unit.Value;
    }
}