namespace game_x.domain.Exceptions;

public sealed class InsufficientBalanceException : DomainException
{
    public override System.Enum ErrorCode { get; set; } = MessageCode.Accounting.InsufficientBalance;
    public decimal CurrentBalance { get; }
    public decimal RequiredAmount { get; }

    public InsufficientBalanceException(decimal currentBalance, decimal requiredAmount)
        : base($"Insufficient balance. Current: {currentBalance}, Required: {requiredAmount}")
    {
        CurrentBalance = currentBalance;
        RequiredAmount = requiredAmount;
    }
}
