using game_x.application.Features.ChainTransactions.TronUsdtDeposit.Dtos;

namespace game_x.application.Features.ChainTransactions.TronUsdtDeposit.Commands;

public record TronUsdtDepositCommand(
    decimal amount,
    string remark) : ICommand<CreateChainTransactionResponseDto>;
