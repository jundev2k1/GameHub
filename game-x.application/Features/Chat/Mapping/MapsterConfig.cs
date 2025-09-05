using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Features.Chat.Dtos;
using ConversationDto = game_x.application.Features.Chat.Dtos.ConversationDto;

namespace game_x.application.Features.Chat.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<Conversation, ConversationSignalDto>()
            .Map(dest => dest.ConversationId, src => src.PublicId)
            .Map(dest => dest.GuestId, src => src.GuestId ?? String.Empty)
            .Map(dest => dest.CustomerId, src => src.CustomerId ?? String.Empty)
            .Map(dest => dest.CustomerDisplayName, src => src.Customer!.Nickname)
            .Map(dest => dest.CustomerAvatarUrl, src => string.Empty)
            .Map(dest => dest.LastMessageAt, src => src.LastMessageAt)
            .Map(dest => dest.LastMessageId, src => src.Messages.FirstOrDefault()!.PublicId)
            .Map(dest => dest.LastMessagePreview, src => src.Messages.FirstOrDefault()!.Text);
        
        cfg.NewConfig<Conversation, SupportConversationDto>()
            .Map(dest => dest.ConversationId, src => src.PublicId)
            .Map(dest => dest.GuestId, src => src.GuestId ?? String.Empty)
            .Map(dest => dest.CustomerId, src => src.CustomerId ?? String.Empty)
            .Map(dest => dest.CustomerDisplayName, src => src.Customer!.Nickname)
            .Map(dest => dest.CustomerAvatarUrl, src => string.Empty)
            .Map(dest => dest.LastMessageAt, src => src.LastMessageAt)
            .Map(dest => dest.LastMessageId, src => src.Messages.FirstOrDefault()!.PublicId)
            .Map(dest => dest.LastMessagePreview, src => src.Messages.FirstOrDefault()!.Text);
        
        cfg.NewConfig<Conversation, ConversationDto>()
            .Map(dest => dest.ConversationId, src => src.PublicId)
            .Map(dest => dest.LastUserId, src => src.Messages.FirstOrDefault()!.SenderUserId)
            .Map(dest => dest.LastUserName, src => 
                src.Messages.FirstOrDefault().SenderUser != null ? src.Messages.FirstOrDefault()!.SenderUser.Nickname : String.Empty)
            .Map(dest => dest.LastUserAvatarUrl, src => string.Empty)
            .Map(dest => dest.LastMessageAt, src => src.LastMessageAt)
            .Map(dest => dest.LastMessageId, src => src.Messages.FirstOrDefault()!.PublicId)
            .Map(dest => dest.LastMessagePreview, src => src.Messages.FirstOrDefault()!.Text);
        
        cfg.NewConfig<Message, MessageDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.ConversationId, src => src.Conversation.PublicId)
            .Map(dest => dest.ReplyToMessageId, src => src.ReplyToMessage!.PublicId);
    }
}
