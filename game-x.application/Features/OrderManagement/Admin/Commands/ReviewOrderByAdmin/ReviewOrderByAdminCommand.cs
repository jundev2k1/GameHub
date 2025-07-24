namespace game_x.application.Features.OrderManagement.Admin.Commands.ReviewOrderByAdmin;

public record ReviewOrderByAdminCommand(Guid OrderId, string OrderStatus) : ICommand;
