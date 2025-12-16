namespace game_x.application.Features.SystemWallets.DTOs;

public sealed class SystemWalletDto
{
    public SystemWalletType Type { get; set; }
    public decimal Balance { get; set; }
}
