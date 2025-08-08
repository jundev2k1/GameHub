namespace game_x.domain.Entities;

public sealed class UserExtend : BaseEntity<int>, IEntity, IAuditable
{
    public string UserId { get; private set; } = string.Empty;
    public User User { get; private set; } = default!;
    public string GameProviderAccount { get; private set; } = string.Empty;
    public string GameProviderPassword { get; private set; } = string.Empty;
    public string GameProviderNickname { get; private set; } = string.Empty;
    public decimal GameProviderRebateset { get; private set; }

    public static UserExtend Create(
        string userId = "",
        string gameProviderAccount = "",
        string gameProviderPassword = "",
        string gameProviderNickname = "",
        decimal? gameProviderRebateset = null)
    {
        return new UserExtend()
        {
            UserId = userId,
            GameProviderAccount = gameProviderAccount,
            GameProviderPassword = gameProviderPassword,
            GameProviderNickname = gameProviderNickname,
            GameProviderRebateset = gameProviderRebateset ?? default,
        };
    }
}
