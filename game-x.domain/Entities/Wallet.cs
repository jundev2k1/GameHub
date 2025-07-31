namespace game_x.domain.Entities;

public class Wallet: BaseEntity<int>
{
    public Guid PublicId { get; set; }
    public string UserId { get; set; } = String.Empty;
    public User User { get; set; } = null!;
    public NetworkType Network { get; set; }
    public string WalletAddress { get; set; } = String.Empty;
}