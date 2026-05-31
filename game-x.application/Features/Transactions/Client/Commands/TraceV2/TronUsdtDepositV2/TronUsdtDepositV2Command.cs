using game_x.application.Features.Transactions.Dtos;

namespace game_x.application.Features.Transactions.Client.Commands.TraceV2.TronUsdtDepositV2;

public record TronUsdtDepositV2Command(
    decimal Amount, 
    string Note,
    Guid CryptoTokenId,
    PaymentGatewayProvider Provider) : ICommand<DepositChainTransactionResponseDto>;