namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

public record ReactionChangedDto(
    int ConversationId, 
    int MessageId, 
    string Emoji, 
    int Count /* optional: IReadOnlyList<string> UserIds */);