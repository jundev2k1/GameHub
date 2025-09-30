using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Queries.GetConversationDetail;

public record GetConversationDetailQuery(Guid ConvId): IQuery<ConversationDetailDto>;