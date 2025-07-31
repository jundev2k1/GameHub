using System.ComponentModel.DataAnnotations;

namespace game_x.domain.Entities;

public sealed class BalanceTransferLog: BaseEntity<int>
{
    public Guid PublicId { get; set; }
    public string FromUserId { get; set; } = String.Empty;
    public User FromUser { get; set; } = null!;

    public string ToUserId { get; set; } = String.Empty;
    public User ToUser { get; set; } = null!;

    public int CryptoTokenId { get; set; }
    public CryptoToken CryptoToken { get; set; } = null!;

    public decimal Amount { get; set; }
    public decimal Fee { get; set; } // Reserving for the future requires a handling fee
    public string? Note { get; set; }

    [Timestamp]
    public uint Version { get; set; }
}