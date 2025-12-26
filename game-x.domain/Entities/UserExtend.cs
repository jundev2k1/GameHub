namespace game_x.domain.Entities;

public sealed class UserExtend : BaseEntity<string>, IEntity
{
    public User User { get; private set; } = default!;
    public string GameProviderAccount { get; private set; } = string.Empty;
    public string GameProviderPassword { get; private set; } = string.Empty;
    public string GameProviderNickname { get; private set; } = string.Empty;
    public decimal GameProviderRebateset { get; private set; }
    public string GameBaccaratUserId { get; private set; } = string.Empty;
    public string GameBaccaratAccount { get; private set; } = string.Empty;
    public string GameBaccaratPassword { get; private set; } = string.Empty;
    public string GameBaccaratNickname { get; private set; } = string.Empty;
    public string Etl998ProviderAccount { get; private set; } = string.Empty;
    public string Etl998ProviderNickname { get; private set; } = string.Empty;
    public string Etl998ProviderPassword { get; private set; } = string.Empty;
    public int Etl998ProviderTableLimit { get; private set; }
    public string SasSlotAccount { get; private set; } = string.Empty;
    public string SasSlotNickname { get; private set; } = string.Empty;
    
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

    public void UpdateBaccaratAccount(
        string gameBaccaratUserId,
        string gameBaccaratAccount,
        string gameBaccaratPassword,
        string gameBaccaratNickname)
    {
        GameBaccaratUserId = gameBaccaratUserId;
        GameBaccaratAccount = gameBaccaratAccount;
        GameBaccaratPassword = gameBaccaratPassword;
        GameBaccaratNickname = gameBaccaratNickname;
    }
    
    public void UpdateEtl998Account(string account, string nickname, string password, int limitId)
    {
        Etl998ProviderAccount = account;
        Etl998ProviderNickname = nickname;
        Etl998ProviderPassword = password;
        Etl998ProviderTableLimit = limitId;
    }

    public void UpdateSasSlotAccount(string account, string nickname)
    {
        SasSlotAccount = account;
        SasSlotNickname = nickname;
    }
}