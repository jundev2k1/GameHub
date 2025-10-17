using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Client.Commands.TraceV1.TronUsdtDeposit;

public record TronUsdtDepositCommand(decimal Amount, string Note, Guid CryptoTokenId) : ICommand<DepositChainTransactionResponseDto>;