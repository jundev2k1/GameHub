namespace game_x.share.ExternalApi.GameBaccarat.Dtos.Login;

public sealed class GameBaccaratLoginRequest
{
    public string Account { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Gamecode { get; set; } = string.Empty;
}
