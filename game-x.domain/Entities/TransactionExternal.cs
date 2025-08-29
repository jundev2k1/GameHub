namespace game_x.domain.Entities;

public class TransactionExternal: BaseEntity<int>, IAuditable
{
    public Transaction Transaction { get; set; } = null!;
    public string G598Sno { get; private set; } = string.Empty;
    public int GamePlatformId { get; private set; }
    public GamePlatform GamePlatform { get; private set; } = null!;
    
    public static TransactionExternal Create(string g598sno, int gamePlatformId)
    {
        return new TransactionExternal
        {
            G598Sno = g598sno,
            GamePlatformId = gamePlatformId,
        };
    }
}