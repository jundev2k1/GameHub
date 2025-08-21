namespace game_x.domain.Enum;

public enum GameTransactionStatus
{
    /// <summary>The transaction is pending execution</summary>
    Pending = 0,
    /// <summary>Complete the transaction with Game Provider</summary>
    Completed = 1,
    /// <summary>The transaction with Game Provider failed</summary>
    Failed = 2
}