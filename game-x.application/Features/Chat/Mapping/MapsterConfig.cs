using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<Conversation, ConversationDto>()
            .Map(dest => dest.Id, src => src.PublicId);
        
        cfg.NewConfig<Conversation, ConversationQueueItemDto>()
            .Map(dest => dest.ConversationId, src => src.PublicId)
            .Map(dest => dest.CustomerUserId, src => src.CustomerId ?? String.Empty)
            .Map(dest => dest.CustomerDisplayName, src => src.Customer!.Nickname)
            .Map(dest => dest.CustomerAvatarUrl, src => string.Empty)
            .Map(dest => dest.LastMessageAt, src => src.LastMessageAt)
            .Map(dest => dest.LastMessageId, src => src.Messages.FirstOrDefault()!.PublicId)
            .Map(dest => dest.LastMessagePreview, src => src.Messages.FirstOrDefault()!.Text);
        
        cfg.NewConfig<Message, MessageDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.ConversationId, src => src.Conversation.PublicId)
            .Map(dest => dest.ReplyToMessageId, src => src.ReplyToMessage!.PublicId);
    }
}
