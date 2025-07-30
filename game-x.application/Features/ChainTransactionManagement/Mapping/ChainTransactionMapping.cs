using game_x.application.Features.ChainTransactionManagement.Client.Commands.TronUsdtWithdrawal;
using game_x.share.ExternalApi.Uxm.Dtos;

namespace game_x.application.Features.ChainTransactionManagement.Mapping;

public static class ChainTransactionMapping
{
    public static UxmWithdrawalOrderRequest ToUxmWithdrawalOrderRequestData(
        this TronUsdtWithdrawalCommand command,
        string merchantNumber)
    {
        var result = command.Adapt<UxmWithdrawalOrderRequest>();
        return result with {
            MerchantNumber = merchantNumber,
        };
    }
}