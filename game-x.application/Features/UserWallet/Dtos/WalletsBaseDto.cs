namespace game_x.application.Features.UserWallet.Dtos;

public class WalletsBaseDto
{
    public NetworkType Network { get; set; }
    public string NetworkDesc { get; set; } = string.Empty;
    public string WalletAddress { get; set; } = string.Empty;
    public List<TokenBalanceDto> Balances { get; set; } = [];
}