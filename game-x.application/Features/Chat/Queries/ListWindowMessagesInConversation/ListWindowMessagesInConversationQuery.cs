using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListWindowMessagesInConversation;

public record ListWindowMessagesInConversationQuery(
    Guid ConvId,
    Guid AnchorId,
    int? Before,
    int? After,
    WindowAnchorType? Anchor) : IQuery<MessageWindowDto>;