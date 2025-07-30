namespace game_x.application.Features.OrderManagement.Dtos;

public sealed class OrderStatisticsDto
{
    public int TotalOrders { get; set; }

    public Dictionary<string, int> OrdersByStatus { get; set; } = new();
}
