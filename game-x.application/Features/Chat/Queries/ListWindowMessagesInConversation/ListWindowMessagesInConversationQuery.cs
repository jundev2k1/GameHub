using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;

namespace game_x.application.Features.Chat.Queries.ListWindowMessagesInConversation;

public record ListWindowMessagesInConversationQuery(
    Guid ConvId,
    Guid AnchorId,
    int Before,
    int After,
    string Anchor) : IQuery<MessageWindowDto>;