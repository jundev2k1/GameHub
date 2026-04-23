namespace game_x.application.Events.Transactions.OnUxmTransactionCallback;

public record OnUxmTransactionCallbackEvent(
    string? ProviderOrderId,
    string? OrderNumber,
    string? Hash,
    decimal ActualAmount,
    DateTime? CreatedAt,
    DateTime? ConfirmedAt,
    string? Remark): IApplicationEvent;