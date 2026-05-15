namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Rewards;

public sealed class UserInventorySignalDto
{
    public Guid CatalogItemId { get; set; }
    public int Quantity { get; set; }
    public string Code { get; set; } = string.Empty;
}