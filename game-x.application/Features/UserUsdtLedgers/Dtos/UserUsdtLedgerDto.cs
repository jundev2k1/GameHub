namespace game_x.application.Features.UserUsdtLedgers.Dtos;

public record UserUsdtLedgerDto(
    Guid Id,
    string UserId,
    UsdtFlowType FlowType,
    string SourceId,
    decimal ChangeAmount,
    decimal BalanceAfter,
    Guid ChainTransactionId,
    ChainTransactionStatus ChainTransactionStatus,
    string StatusAtEvent,
    NetworkType? Network,
    string? Symbol,
    DateTime CreatedAt,
    DateTime UpdatedAt);