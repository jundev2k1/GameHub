namespace game_x.share.ExternalApi.GameBaccarat.Dtos.Register;

public sealed class GameBaccaratRegisterRequest
{
    public string Account { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Nickname { get; set; } = string.Empty;
}