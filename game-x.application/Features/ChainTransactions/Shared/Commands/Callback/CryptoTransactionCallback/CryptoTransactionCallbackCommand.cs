
using game_x.share.ExternalApi.Uxm.Dtos;

namespace game_x.application.Features.ChainTransactions.Shared.Commands.Callback.CryptoTransactionCallback;

public record CryptoTransactionCallbackCommand(CryptoCallbackRequest Data, string Signature) : ICommand<Unit>;
