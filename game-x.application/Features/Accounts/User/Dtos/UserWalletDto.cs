namespace game_x.application.Features.Accounts.User.Dtos;

public sealed class UserWalletDto
{
    public List<UserWalletInternalItemDto> InternalWallets { get; set; } = [];
    public decimal? InternalTotalAmount { get; set; }
    public List<UserWalletExternalItemDto> ExternalWallets { get; set; } = [];
    public decimal? ExternalTotalAmount { get; set; }
}

public sealed class UserWalletInternalItemDto
{
    public Guid WalletId { get; set; }
    public Guid CryptoTokenId { get; set; }
    public string Symbol { get; set; } = string.Empty;
    public NetworkType Network { get; set; }
    public decimal Amount { get; set; }
    public decimal FrozenAmount { get; set; }
    public decimal TotalAmount => Amount + FrozenAmount;
}

public sealed class UserWalletExternalItemDto
{
    public Guid PlatformId { get; set; }
    public string PlatformName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}
