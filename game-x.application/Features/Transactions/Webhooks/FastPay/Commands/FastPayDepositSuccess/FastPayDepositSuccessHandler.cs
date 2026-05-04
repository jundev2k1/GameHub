using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Events.Transactions.OnUxmTransactionCallback;

namespace game_x.application.Features.Transactions.Webhooks.FastPay.Commands.FastPayDepositSuccess;

public sealed class FastPayDepositSuccessHandler(
    IAsymmetricCryptoService asymmetricCryptoService,
    IAsymmetricKeyCacheService asymmetricKeyCacheService,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<FastPayDepositSuccessCommand>
{
    public async Task<Unit> Handle(FastPayDepositSuccessCommand request, CancellationToken ct = default)
    {
        var (requestData, signature) = request;

        // Verify UXM signature
        var isValid = asymmetricCryptoService.VerifySignature(
            asymmetricKeyCacheService.FastPayPublicKey,
            requestData,
            signature);
        if (!isValid) throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");

        var @event = new OnUxmTransactionCallbackEvent(
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
