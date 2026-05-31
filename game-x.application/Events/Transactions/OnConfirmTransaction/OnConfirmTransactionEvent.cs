namespace game_x.application.Events.Transactions.OnConfirmTransaction;

public record OnConfirmTransactionEvent(
    string? ProviderOrderId,
    string? OrderNumber,
    string? Hash,
    decimal ActualAmount,
    DateTime? CreatedAt,
    DateTime? ConfirmedAt,
    string? Remark): IApplicationEvent;