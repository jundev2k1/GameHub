namespace game_x.share.ExternalApi.Uxm.Dtos;

public record CreateOrderSellRequestData(
    string MerchantNumber,
    string MerchantOrderId,
    string MemberId,
    decimal FiatAmount,
    string FiatType,
    string PayeeBankName,
    string PayeeBranchCode,
    string PayeeName,
    string PayeeAccountNumber);
