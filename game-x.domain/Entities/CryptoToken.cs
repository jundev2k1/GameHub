namespace game_x.domain.Entities;

public class CryptoToken : BaseEntity<int>
{
    public Guid PublicId { get; set; }
    public string Symbol { get; set; } = string.Empty;  // e.g. USDT, ETH, TRX
    public NetworkType Network { get; set; }  // e.g. Tron, Ethereum
    public string ContractAddress { get; set; } = string.Empty;  // Token contract address (null indicates native currency)
    public CryptoTokenStatus Status { get; set; } = CryptoTokenStatus.Active;
}