using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Events.Transactions.OnFailedTransaction;

namespace game_x.application.Features.Transactions.Webhooks.FastPay.Commands.FastPayWithdrawalFailed;

public sealed class FastPayWithdrawalFailedHandler(
    IAsymmetricCryptoService asymmetricCryptoService,
    IAsymmetricKeyCacheService asymmetricKeyCacheService,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<FastPayWithdrawalFailedCommand>
{
    public async Task<Unit> Handle(FastPayWithdrawalFailedCommand request, CancellationToken ct = default)
    {
        var (requestData, signature) = request;

        // Verify UXM signature
        var isValid = asymmetricCryptoService.VerifySignature(
            asymmetricKeyCacheService.FastPayPublicKey,
            requestData,
            signature);
        if (!isValid) throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");

        var @event = new OnFailedTransactionEvent(
            requestData.OrderUid,
            requestData.OrderNumber,
            requestData.Type,
            requestData.Status,
            requestData.Amount,
            requestData.FailureCategory,
            requestData.FailureCode,
            requestData.FailureMessage,
            requestData.FailedAt);
        await eventDispatcher.Publish(@event, ct);

        return Unit.Value;
    }
}
