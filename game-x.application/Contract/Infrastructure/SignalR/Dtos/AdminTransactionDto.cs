namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public record AdminTransactionDto(Guid TransactionId, string Type, string Status);
