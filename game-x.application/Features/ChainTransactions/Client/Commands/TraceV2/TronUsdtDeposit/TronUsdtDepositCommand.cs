using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Client.Commands.TraceV2.TronUsdtDeposit;

public record TronUsdtDepositCommand(
    decimal Amount, 
    string Note,
    Guid CryptoTokenId,
    PaymentGatewayProvider Provider) : ICommand<DepositChainTransactionResponseDto>;