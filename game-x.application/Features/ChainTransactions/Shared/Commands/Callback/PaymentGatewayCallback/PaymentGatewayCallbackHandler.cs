using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Events.OnUxmTransactionCallback;
using game_x.share.Settings;
using Microsoft.Extensions.Options;

namespace game_x.application.Features.ChainTransactions.Shared.Commands.Callback.PaymentGatewayCallback;

public sealed class PaymentGatewayCallbackHandler(
    IAsymmetricCryptoService asymmetricCryptoService,
    IOptions<GameXSettings> gameXSettings,
    IApplicationEventDispatcher eventDispatcher) : ICommandHandler<PaymentGatewayCallbackCommand, Unit>
{
    public async Task<Unit> Handle(PaymentGatewayCallbackCommand request,
        CancellationToken ct = default)
    {
        var (requestData, signature) = request;

        // Verify Payment Gateway signature
        var apiKey = gameXSettings.Value.PaymentGatewayApiKey;
        bool isValid = asymmetricCryptoService.PaymentGatewayVerifySignature(apiKey, requestData, signature);
        if (!isValid) throw new BadRequestException(MessageCode.System.TokenGenerationFailed, "Invalid signature.");
        
        await eventDispatcher.Publish(new OnUxmTransactionCallbackEvent(
            ProviderOrderId: requestData.ProviderOrderId,
            Hash: requestData.TransactionHash,
            OrderNumber: requestData.MerchantOrderId,
            ActualAmount: requestData.FinalAmount,
            CreatedAt: requestData.CreatedAt,
            ConfirmedAt: requestData.CompletedAt,
            Remark: requestData.Remark), ct);

        return Unit.Value;
    }
}