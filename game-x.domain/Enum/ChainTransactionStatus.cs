namespace game_x.domain.Enum;

public enum ChainTransactionStatus
{
    /// <summary>The transaction is pending execution</summary>
    Pending = 0,
    /// <summary>Admin approves the transaction; withdrawal only</summary>
    Approved = 1,
    /// <summary>Complete the transaction with Uxm service</summary>
    Completed = 2,
    /// <summary>Admin rejects the transaction; withdrawal only</summary>
    Rejected = 3,
    /// <summary>The transaction with Uxm failed</summary>
    Failed = 4
}