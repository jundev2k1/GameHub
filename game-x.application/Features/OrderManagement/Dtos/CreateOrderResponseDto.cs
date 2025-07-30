namespace game_x.application.Features.OrderManagement.Dtos;

public sealed class CreateOrderResponseDto
{
    public string OrderUid { get; set; } = default!;
    public decimal Amount { get; set; } = default!;
    public string To { get; set; }
}
