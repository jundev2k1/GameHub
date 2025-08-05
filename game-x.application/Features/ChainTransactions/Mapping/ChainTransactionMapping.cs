using game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtWithdrawal;
using game_x.share.ExternalApi.Uxm.Dtos;

namespace game_x.application.Features.ChainTransactions.Mapping;

public static class ChainTransactionMapping
{
    public static UxmWithdrawalOrderRequest ToUxmWithdrawalOrderRequestData(
        this TronUsdtWithdrawalCommand command,
        string merchantNumber,
        string orderNumber)
    {
        var result = command.Adapt<UxmWithdrawalOrderRequest>();
        return result with {
            MerchantNumber = merchantNumber,
            OrderNumber = orderNumber
        };
    }
}