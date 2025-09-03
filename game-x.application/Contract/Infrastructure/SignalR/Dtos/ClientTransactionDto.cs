namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public record ClientTransactionDto(
    Guid TransactionId, 
    string Type, 
    string Status,
    decimal? ActualAmount,
    decimal? BalanceAfter,
    DateTime? ConfirmedAt,
    string? Hash);