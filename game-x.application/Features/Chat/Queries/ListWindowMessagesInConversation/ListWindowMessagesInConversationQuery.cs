using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.ListWindowMessagesInConversation;

public record ListWindowMessagesInConversationQuery(
    Guid ConvId,
    Guid AnchorId,
    int Before,
    int After,
    string Anchor) : IQuery<MessageWindowDto>;