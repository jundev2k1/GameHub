using game_x.share.ExternalApi.Uxm.Dtos.Webhooks.CryptoCallback;

namespace game_x.application.Features.Transactions.Webhooks.Uxm.Commands.CryptoTransactionCallback;

public record CryptoTransactionCallbackCommand(
    CryptoCallbackRequest Data,
    string Signature) : ICommand;
