namespace game_x.application.Features.UserUsdtLedgers.Dtos;

public record UserUsdtLedgerDetailDto(
    Guid Id,
    string UserId,
    DateTime Timestamp,
    UsdtFlowType FlowType,
    string SourceId,
    decimal Amount,
    decimal Fee,
    decimal ChangeAmount,
    decimal BalanceAfter,
    Guid ChainTransactionId,
    string Meta,
    DateTime CreatedAt,
    DateTime UpdatedAt);