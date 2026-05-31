namespace game_x.application.Contract.Infrastructure.SignalR.Dtos;

public record UserKycDto(
    KycStatus? Status,
    string? Reason,
    string? Details);