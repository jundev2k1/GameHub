using System.ComponentModel.DataAnnotations;

namespace game_x.domain.Entities;

public sealed class UserBalance: BaseEntity<int>, IAuditable
{
    public Guid PublicId { get; set; }
    public string UserId { get; set; } = String.Empty;
    public User User { get; set; } = null!;
    public int CryptoTokenId { get; set; }
    public CryptoToken CryptoToken { get; set; } = null!;
    public decimal Amount { get; set; } // Available balance
    public decimal FrozenAmount { get; set; }
    public decimal TotalAmount => Amount + FrozenAmount; //  Additional support for backend queries and reporting

    [Timestamp]
    public uint Version { get; set; }
}