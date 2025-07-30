namespace game_x.domain.ValueObjects;

public sealed class ChainTransactionType
{
    public string Value { get; }
    public int UxmValue { get; }

    private static string DepositValue => "DEPOSIT";
    private static string WithdrawalValue => "WITHDRAWAL";
    private static int UxmDepositValue => 0;
    private static int UxmWithdrawalValue => 1;
    public static ChainTransactionType Deposit => Of(DepositValue);
    public static ChainTransactionType Withdrawal => Of(WithdrawalValue);
    private static readonly HashSet<string> ValidValues = [DepositValue, WithdrawalValue];

    private ChainTransactionType(string value)
    {
        Value = value;
        UxmValue = GetUxmValue(value);
    }

    private int GetUxmValue(string value)
    {
        if (value == DepositValue)
            return UxmDepositValue;
        if (value == WithdrawalValue)
            return UxmWithdrawalValue;
        throw new ArgumentException($"OrderType must be '{DepositValue}' or '{WithdrawalValue}'.");
    }

    public static ChainTransactionType Of(string value)
    {
        if (!IsValid(value))
            throw new ArgumentException($"OrderType must be '{DepositValue}' or '{WithdrawalValue}'.");
        return new ChainTransactionType(value.ToUpperInvariant());
    }

    public override bool Equals(object? obj) =>
        (obj != null) && (obj is ChainTransactionType type) && (Value == type.Value);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    public static bool IsValid(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;
        return ValidValues.Contains(value.ToUpperInvariant());
    }

    public static ChainTransactionType GetChainTransactionType(int value)
    {
        if (value == UxmDepositValue)
            return Deposit;
        if (value == UxmWithdrawalValue)
            return Withdrawal;
        throw new ArgumentException();
    }
}
