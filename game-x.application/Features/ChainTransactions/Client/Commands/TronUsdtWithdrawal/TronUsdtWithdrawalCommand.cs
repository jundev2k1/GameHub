namespace game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtWithdrawal;

public record TronUsdtWithdrawalCommand(
    string To,
    decimal Amount,
    string? Note = null) : ICommand<Unit>;