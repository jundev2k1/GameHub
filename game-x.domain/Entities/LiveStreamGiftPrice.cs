namespace game_x.domain.Entities;

public sealed class LiveStreamGiftPrice : BaseEntity<object>, IAuditable
{
    public int LiveStreamGiftId { get; private set; }
    public LiveStreamGift LiveStreamGift { get; private set; } = null!;
    public int CryptoTokenId { get; private set; }
    public CryptoToken CryptoToken { get; private set; } = null!;
    public decimal TokenCost { get; private set; }
    public bool IsActive { get; private set; } = true;

    public static LiveStreamGiftPrice Create(
        int liveStreamGiftId,
        int cryptoTokenId,
        decimal tokenCost)
    {
        if (tokenCost <= 0)
            throw new ArgumentException("Token cost must be greater than zero.", nameof(tokenCost));

        return new LiveStreamGiftPrice
        {
            LiveStreamGiftId = liveStreamGiftId,
            CryptoTokenId = cryptoTokenId,
            TokenCost = tokenCost,
        };
    }

    public void UpdatePrice(decimal tokenCost)
    {
        if (tokenCost <= 0)
            throw new ArgumentException("Token cost must be greater than zero.", nameof(tokenCost));

        TokenCost = tokenCost;
    }

    public void Enable() => IsActive = true;

    public void Disable() => IsActive = false;
}
