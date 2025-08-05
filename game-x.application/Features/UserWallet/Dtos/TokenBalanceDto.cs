namespace game_x.application.Features.UserWallet.Dtos;

public class TokenBalanceDto
{
    public string Symbol { get; set; } = String.Empty;
    public decimal Amount { get; set; }
    public decimal FrozenAmount { get; set; }
    public decimal TotalAmount { get; set; }
}