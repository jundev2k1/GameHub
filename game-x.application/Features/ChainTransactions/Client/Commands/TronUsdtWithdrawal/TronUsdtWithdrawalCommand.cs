using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtWithdrawal;

public record TronUsdtWithdrawalCommand(
    string To,
    decimal Amount,
    Guid CryptoTokenId,
    string? Note = null) : ICommand<ChainTransactionDto>;