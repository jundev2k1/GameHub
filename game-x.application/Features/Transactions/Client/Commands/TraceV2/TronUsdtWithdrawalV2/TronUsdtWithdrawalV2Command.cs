using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Features.Transactions.Client.Commands.TraceV2.TronUsdtWithdrawalV2;

public record TronUsdtWithdrawalV2Command(
    string To,
    decimal Amount,
    Guid CryptoTokenId,
    PaymentGatewayProvider Provider,
    string? Note = null) : ICommand<ListTransactionInternalDto>;