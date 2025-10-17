using game_x.share.ExternalApi.PaymentGateway.Dtos;

namespace game_x.application.Features.ChainTransactions.Shared.Commands.Callback.PaymentGatewayCallback;

public record PaymentGatewayCallbackCommand(PaymentGatewayCallbackRequest Data, string Signature) : ICommand<Unit>;
