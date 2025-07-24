namespace game_x.application.Features.Auth.Dtos;

public class StaffLoginDto
{
    public string UserName { get; set; } = null!;
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
    public string UserId { get; set; } = null!;
    public Guid CounterId { get; set; }
    public Guid SessionKey { get; set; }
    public List<string> Roles { get; set; } = [];
}
