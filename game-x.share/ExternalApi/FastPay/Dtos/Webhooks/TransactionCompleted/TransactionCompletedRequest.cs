namespace game_x.share.ExternalApi.FastPay.Dtos.Webhooks.TransactionCompleted;

public record TransactionCompletedRequest(
    string? OrderUid,
    string? OrderNumber,
    string Type,
    string Status,
    string? Hash,
    decimal ActualAmount,
    DateTime? CreatedAt,
    DateTime? ConfirmedAt,
    string? Remark);
