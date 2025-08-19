namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record MemberDelivery(
    string UserId, 
    DateTime LastDeliveredAt);