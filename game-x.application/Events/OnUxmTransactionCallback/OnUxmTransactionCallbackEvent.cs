namespace game_x.application.Events.OnUxmTransactionCallback;

public record OnUxmTransactionCallbackEvent(
    string? OrderUid,
    string? OrderNumber,
    string? Hash,
    decimal ActualAmount,
    DateTime? CreatedAt,
    DateTime? ConfirmedAt,
    string? Remark): IApplicationEvent;