namespace game_x.application.Features.Auth.Dtos;

public class AdminLoginDto
{
    public string UserName { get; set; } = default!;
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
    public string UserId { get; set; } = default!;
    public List<string> Roles { get; set; } = [];
}