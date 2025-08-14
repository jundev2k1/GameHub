namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public record UserBankAccountDto(
    UserBankAccountStatus? Status,
    string? Reason,
    string? Details);