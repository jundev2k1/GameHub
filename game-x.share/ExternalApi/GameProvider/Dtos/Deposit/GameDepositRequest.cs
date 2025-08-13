namespace game_x.share.ExternalApi.GameProvider.Dtos.Deposit;

public sealed class GameDepositRequest
{
    public string Account { get; set; } = string.Empty;
    public decimal Quota { get; set; }
    public string Sno { get; set; } = string.Empty;
}