namespace game_x.share.ExternalApi.Uxm.Dtos;

public record CreateOrderBuyRequestData(
    string MerchantNumber,
    string MerchantOrderId,
    string MemberId,
    string PayerBankAccountName,
    decimal FiatAmount,
    string FiatType);
