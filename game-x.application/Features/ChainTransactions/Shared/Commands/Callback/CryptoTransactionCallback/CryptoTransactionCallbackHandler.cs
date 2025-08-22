using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Events.OnUxmTransactionCallback;

namespace game_x.application.Features.ChainTransactions.Shared.Commands.Callback.CryptoTransactionCallback;

public sealed class CryptoTransactionCallbackHandler(
    IAsymmetricCryptoService asymmetricCryptoService,
    IAsymmetricKeyCacheService asymmetricKeyCacheService,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<CryptoTransactionCallbackCommand, Unit>
{
    public async Task<Unit> Handle(CryptoTransactionCallbackCommand request,
        CancellationToken ct = default)
    {
        var (requestData, signature) = request;

        // Verify UXM signature
        var uxmPublicKey = asymmetricKeyCacheService.UxmPublicKey;
        var isValid = asymmetricCryptoService.VerifySignature(uxmPublicKey, requestData, signature);
        if (!isValid)
            throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");

        await eventDispatcher.Publish(new OnUxmTransactionCallbackEvent(
            OrderUid: requestData.OrderUid,
            Hash: requestData.Hash,
            OrderNumber: requestData.OrderNumber,
            ActualAmount: requestData.ActualAmount,
            CreatedAt: requestData.CreatedAt,
            ConfirmedAt: requestData.ConfirmedAt,
            Remark: requestData.Remark), ct);

        return Unit.Value;
    }
}
