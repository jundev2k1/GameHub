namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public record ClientLedgerDto(
    Guid LedgerId,
    string? Status);