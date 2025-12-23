using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Features.Transactions.Client.Commands.TraceV1.TronUsdtDeposit;

public record TronUsdtDepositCommand(decimal Amount, string Note, Guid CryptoTokenId) : ICommand<DepositChainTransactionResponseDto>;