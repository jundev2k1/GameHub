using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Features.Chat.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<ConversationItemDto, ConversationDto>()
            .Map(dest => dest.ConversationId, src => src.Id)
            .Map(dest => dest.CustomerAvatarUrl, src => string.Empty)
            .Map(dest => dest.LastSenderRole, src => src.LastMessage != null ? src.LastMessage.SenderRole : RoleInConversation.Member)
            .Map(dest => dest.LastUserId, src => src.LastMessage != null ? src.LastMessage.SenderActorId : null)
            .Map(dest => dest.LastUserName, src => src.LastMessage != null ? src.LastMessage.SenderName : string.Empty)
            .Map(dest => dest.LastUserAvatarUrl, src => string.Empty)
            .Map(dest => dest.LastMessageId, src => src.LastMessage != null ? src.LastMessage.PublicId : Guid.Empty)
            .Map(dest => dest.LastMessageText, src => 
                src.LastMessage != null ? src.LastMessage.Kind == MessageKind.Text ? src.LastMessage.Text : "[Attachment]" : null)
            .Map(dest => dest.LastMessageKind, src => src.LastMessage != null ? src.LastMessage.Kind : MessageKind.Text);
        
        cfg.NewConfig<Message, MessageDto>()
            .Map(dest => dest.ConversationId, src => src.Conversation.PublicId)
            .Map(dest => dest.ReplyToMessageId, src => src.ReplyToMessage!.PublicId)
            .Map(dest => dest.DirectMentions, src => src.Attachments.Adapt<List<DirectMention>>())
            .Map(dest => dest.Attachments, src => src.Attachments.Adapt<List<MessageAttachmentDto>>());
  
        cfg.NewConfig<MessageDto, ListedMessageDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.SenderUserNickname, src => src.SenderUser!.Nickname);
        
        cfg.NewConfig<MessageAttachment, MessageAttachmentDto>()
            .Map(dest => dest.Attachment, src => src.MediaFile);
    }
}