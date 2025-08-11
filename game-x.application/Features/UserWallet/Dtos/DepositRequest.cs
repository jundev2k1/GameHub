namespace game_x.application.Features.UserWallet.Dtos;

public class DepositRequest
{
    public string WalletAddress { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string TokenSymbol { get; set; } = string.Empty;
    public NetworkType Network { get; set; }
    public string? TransactionHash { get; set; }
}