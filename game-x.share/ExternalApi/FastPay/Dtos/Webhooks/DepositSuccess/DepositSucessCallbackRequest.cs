using game_x.share.ExternalApi.FastPay.Enums;

namespace game_x.share.ExternalApi.FastPay.Dtos.Webhooks.DepositSuccess;

public record DepositSucessCallbackRequest(
    string? OrderUid,
    string? OrderNumber,
    FastPayOrderType Type,
    FastPayOrderStatus Status,
    string? Hash,
    decimal ActualAmount,
    DateTime? CreatedAt,
    DateTime? ConfirmedAt,
    string? Remark);
