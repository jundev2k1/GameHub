namespace game_x.domain.Enum;

public enum TransactionType
{
    /// <summary>
    /// Initialize the ledger to serve as the foundation for transactions throughout the lifecycle of the user account
    /// </summary>
    Init = 0,
    Deposit = 1,
    Withdrawal = 2,
}