namespace game_x.application.Features.Accounts.User.Dtos;

public sealed class VerificationCreatedDto
{
    public string Type { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NickName { get; set; } = string.Empty;
}
