namespace game_x.domain.Exceptions;

public sealed class BusinessRuleViolationException : DomainException
{
    public override System.Enum ErrorCode { get; set; } = MessageCode.System.InvalidOperation;

    public BusinessRuleViolationException(string ruleDescription)
        : base($"Business rule violated: {ruleDescription}")
    {
    }
    public BusinessRuleViolationException(System.Enum errorCode)
        : base(errorCode.ToMessage())
    {
        ErrorCode = errorCode;
    }
    public BusinessRuleViolationException(System.Enum errorCode, string? message)
        : base(message)
    {
        ErrorCode = errorCode;
    }
}
