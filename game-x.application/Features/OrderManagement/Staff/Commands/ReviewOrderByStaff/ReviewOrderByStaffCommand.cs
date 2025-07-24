namespace game_x.application.Features.OrderManagement.Staff.Commands.ReviewOrderByStaff;

public record ReviewOrderByStaffCommand : ICommand
{
    public Guid OrderId { get; init; }
    public required string OrderStatus { get; init; }
}