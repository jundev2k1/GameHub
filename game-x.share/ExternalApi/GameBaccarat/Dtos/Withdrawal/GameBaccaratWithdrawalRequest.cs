namespace game_x.share.ExternalApi.GameBaccarat.Dtos.Withdrawal;

public sealed class GameBaccaratWithdrawalRequest
{
    public string UserId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Sno { get; set; } = string.Empty;
}
