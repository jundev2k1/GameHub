namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public sealed class AdminVerificationCreatedDto
{
    public string? Type { get; set; }
    public string? Email { get; set; }
    public string? NickName { get; set; }
    public string? AccountName { get; set; }
}
