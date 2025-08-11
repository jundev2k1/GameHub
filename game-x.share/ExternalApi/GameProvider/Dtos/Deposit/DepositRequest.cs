namespace game_x.share.ExternalApi.GameProvider.Dtos.Deposit;

public class DepositRequest
{
    public string Account { get; set; } = string.Empty;
    public decimal Quota { get; set; }
    public string Sno { get; set; } = string.Empty;
}