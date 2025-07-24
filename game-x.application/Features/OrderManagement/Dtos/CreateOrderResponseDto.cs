namespace game_x.application.Features.OrderManagement.Dtos;

public sealed class CreateOrderResponseDto
{
    public string EntryCode { get; set; } = default!;
    public string RedirectUrl { get; set; } = default!;
    public bool IsValid { get; set; }
}
