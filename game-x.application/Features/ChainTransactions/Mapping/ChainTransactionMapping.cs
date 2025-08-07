using game_x.application.Features.ChainTransactions.Client.Commands.TronUsdtWithdrawal;
using game_x.share.ExternalApi.Uxm.Dtos;

namespace game_x.application.Features.ChainTransactions.Mapping;

public static class ChainTransactionMapping
{
    public static UxmWithdrawalOrderRequest ToUxmWithdrawalOrderRequestData(
        this ChainTransaction transaction,
        string merchantNumber)
    {
        var result = transaction.Adapt<UxmWithdrawalOrderRequest>();
        return result with {
            MerchantNumber = merchantNumber,
            OrderNumber = transaction.OrderNumber,
            Amount =  transaction.Amount,
            To = transaction.ToAddress ?? string.Empty,
            Remark = transaction.Note ?? string.Empty
        };
    }
}