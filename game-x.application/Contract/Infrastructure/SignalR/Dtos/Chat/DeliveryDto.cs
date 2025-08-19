namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record DeliveryDto(
    int ConversationId, 
    IReadOnlyList<MemberDelivery> Members);