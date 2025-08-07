namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public record ClientTransactionDto(Guid TransactionId, string Type, string Status);