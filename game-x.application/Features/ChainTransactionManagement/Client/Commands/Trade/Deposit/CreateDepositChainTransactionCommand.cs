using game_x.application.Features.ChainTransactionManagement.Dtos;

namespace game_x.application.Features.ChainTransactionManagement.Client.Commands.Trade.Deposit;

public record CreateDepositChainTransactionCommand(
    // string MerchantNumber,
    decimal amount,
    // string orderNumber,
    //  string userId,
    string remark) : ICommand<CreateChainTransactionResponseDto>;
