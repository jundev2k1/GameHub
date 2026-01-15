using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Features.Transactions.Client.Commands.TraceV1.TronUsdtWithdrawal;

public record TronUsdtWithdrawalCommand(
    string To,
    decimal Amount,
    Guid CryptoTokenId,
    string Code,
    string? Note = null) : ICommand<ListTransactionInternalDto>;