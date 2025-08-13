namespace game_x.share.ExternalApi.GameProvider.Dtos.Withdrawal;

public sealed class GameWithdrawalRequest
{
    public string Account { get; set; } = string.Empty;
    public decimal Quota { get; set; }
    public string Sno { get; set; } = string.Empty;
}