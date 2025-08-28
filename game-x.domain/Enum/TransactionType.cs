namespace game_x.domain.Enum;

public enum TransactionType
{
    /// <summary>
    /// Initialize the ledger to serve as the foundation for transactions throughout the lifecycle of the user account
    /// </summary>
    Init = 0,
    /// <summary>Deposit transaction with the Uxm service</summary>
    UxmDeposit = 1,
    /// <summary>Withdrawal transaction with the Uxm service</summary>
    UxmWithdrawal = 2,
    /// <summary>Deposit transaction with a third-party game provider</summary>
    GameDeposit = 3,
    /// <summary>Withdrawal transaction with a third-party game provider</summary>
    GameWithdrawal = 4,
}