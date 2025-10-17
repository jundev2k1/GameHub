using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Client.Commands.TraceV2.TronUsdtWithdrawal;

public record TronUsdtWithdrawalCommand(
    string To,
    decimal Amount,
    Guid CryptoTokenId,
    PaymentGatewayProvider Provider,
    string? Note = null) : ICommand<ListTransactionInternalDto>;