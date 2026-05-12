namespace game_x.share.ExternalApi.FastPay.Dtos.Webhooks.TransactionFailed;

public record TransactionFailedRequest(
    string? OrderUid,
    string? OrderNumber,
    string Type,
    string Status,
    decimal Amount,
    string FailureCategory,
    string FailureCode,
    string FailureMessage,
    string FailedAt);
