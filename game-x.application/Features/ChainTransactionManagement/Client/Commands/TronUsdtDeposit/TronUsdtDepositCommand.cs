using game_x.application.Features.ChainTransactionManagement.Dtos;

namespace game_x.application.Features.ChainTransactionManagement.Client.Commands.TronUsdtDeposit;

public record TronUsdtDepositCommand(
    decimal amount,
    string remark) : ICommand<CreateChainTransactionResponseDto>;
