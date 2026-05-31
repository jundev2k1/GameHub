namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public class VerificationNotificationDto
{
    public string? CurrencyCode { get; set; }
    public string? Type { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool? IsVerified { get; set; }
}