namespace game_x.application.Features.OrderManagement.Admin.Commands.UpdateOrderInfoByAdmin;

public record UpdateOrderInfoByAdminCommand(
    Guid OrderId,
    int Quantity,
    int PriceOfUnit,
    string CurrencyUnit,
    string OrderStatus,
    string Notes) : ICommand;