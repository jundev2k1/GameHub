using game_x.application.Features.ChainTransactions.Dtos;

namespace game_x.application.Features.ChainTransactions.Client.Commands.TraceV2.TronUsdtDepositV2;

public record TronUsdtDepositV2Command(
    decimal Amount, 
    string Note,
    Guid CryptoTokenId,
    PaymentGatewayProvider Provider) : ICommand<DepositChainTransactionResponseDto>;