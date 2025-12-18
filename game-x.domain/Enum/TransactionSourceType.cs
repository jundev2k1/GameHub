namespace game_x.domain.Enum;

public enum TransactionSourceType
{
    /// <summary>The transaction from GameX internal system</summary>
    GameX = 0,
    /// <summary>The transaction takes place with the Uxm service</summary>
    Uxm = 1,
    /// <summary>The transaction takes place with the third-party game 598 provider</summary>
    G598SnoGameProvider = 2,
    /// <summary>The transaction takes place with the third-party game Baccarat provider</summary>
    BaccaratGameProvider = 3
}
