using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtDeposit;

public record TronUsdtDepositCommand(
    decimal Amount,
    string Note,
    Guid CryptoTokenId) : ICommand<CreateChainTransactionResponseDto>;
