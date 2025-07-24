namespace game_x.application.Features.CounterManagement.Dtos;

public sealed class CounterDto
{
    public string CounterId { get; set; } = string.Empty;
    public string CounterNumber { get; set; } = string.Empty;
    public string CounterName { get; set; } = string.Empty;
    public string CounterToken { get; set; } = string.Empty;
    public CounterStatus Status { get; set; }
    public string Location { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
