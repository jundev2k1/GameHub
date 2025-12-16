namespace game_x.domain.Entities;

public class TransactionExternal: BaseEntity<int>
{
    public Transaction Transaction { get; set; } = null!;
    public string SerialNumber { get; private set; } = string.Empty;
    public int GamePlatformId { get; private set; }
    public GamePlatform GamePlatform { get; private set; } = null!;
    
    public static TransactionExternal Create(string sno, int gamePlatformId)
    {
        return new TransactionExternal
        {
            SerialNumber = sno,
            GamePlatformId = gamePlatformId,
        };
    }
}