namespace game_x.domain.Entities;

public sealed class UserExtend : BaseEntity<string>, IEntity
{
    public User User { get; private set; } = default!;
    public string GameProviderAccount { get; private set; } = string.Empty;
    public string GameProviderPassword { get; private set; } = string.Empty;
    public string GameProviderNickname { get; private set; } = string.Empty;
    public decimal GameProviderRebateset { get; private set; }

    public static UserExtend Create() => new ();

    public void UpdateG598Account(
        string gameProviderAccount,
        string gameProviderPassword,
        string gameProviderNickname,
        decimal? gameProviderRebateset = null)
    {
        GameProviderAccount = gameProviderAccount;
        GameProviderPassword = gameProviderPassword;
        GameProviderNickname = gameProviderNickname;
        GameProviderRebateset = gameProviderRebateset ?? default;
    }
}
