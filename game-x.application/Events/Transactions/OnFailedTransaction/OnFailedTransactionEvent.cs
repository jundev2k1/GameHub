namespace game_x.application.Events.Transactions.OnFailedTransaction;

public record OnFailedTransactionEvent(
    string? OrderUid,
    string? OrderNumber,
    string Type,
    string Status,
    decimal Amount,
    string FailureCategory,
    string FailureCode,
    string FailureMessage,
    string FailedAt) : IApplicationEvent;