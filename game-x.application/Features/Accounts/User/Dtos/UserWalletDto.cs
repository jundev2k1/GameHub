namespace game_x.application.Features.Accounts.User.Dtos;

public sealed class UserWalletDto
{
    public List<UserWalletInternalItemDto> InternalWallets { get; set; } = [];
    public decimal? InternalTotalAmount => InternalWallets.Sum(x => x.TotalAmount);
    public decimal? InternalAvailableAmount => InternalWallets.Sum(x => x.Amount);
    public decimal? InternalLockedAmount => InternalWallets.Sum(x => x.FrozenAmount);
    public List<UserWalletExternalItemDto> ExternalWallets { get; set; } = [];
    public decimal? ExternalTotalAmount => ExternalWallets.Sum(x => x.TotalAmount);
    public decimal? ExternalAvailableAmount => ExternalWallets.Sum(x => x.Amount);
    public decimal? ExternalLockedAmount => ExternalWallets.Sum(x => x.LockedAmount);
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
    public decimal LockedAmount { get; set; }
    public decimal TotalAmount => Amount + LockedAmount;
    public DateTime LastSyncAt { get; set; }
}
