namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Transactions;

public record ClientTransactionDto(
    Guid TransactionId, 
    string Type, 
    string Status,
    decimal? ActualAmount,
    decimal? BalanceAfter,
    DateTime? ConfirmedAt,
    string? Hash);