using game_x.share.ExternalApi.Uxm.Dtos;

namespace game_x.application.Features.ChainTransactionManagement.Shared.Commands.Callback.Crypto;

public record UpdateChainTransactionCallbackCommand(CryptoCallbackRequest Data, string Signature) : ICommand<UpdateChainTransactionCallbackResult>;

public record UpdateChainTransactionCallbackResult(string Message);
