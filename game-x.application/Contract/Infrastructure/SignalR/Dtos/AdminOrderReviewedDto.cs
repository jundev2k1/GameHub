namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public sealed class AdminOrderReviewedDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public int UnderReviewCount { get; set; }
}
