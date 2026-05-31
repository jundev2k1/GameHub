using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Features.Chat.Dtos;
using game_x.share.Extensions;

namespace game_x.application.Features.Chat.Mapping;

public sealed class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig cfg)
    {
        cfg.NewConfig<Conversation, ConversationDto>()
            .Map(dest => dest.ConversationId, src => src.PublicId)
            .Map(dest => dest.CustomerAvatarUrl, src => string.Empty)
            .Map(dest => dest.LastSenderRole, src => src.Messages.FirstOrDefault()!.SenderRole)
            .Map(dest => dest.LastUserId, src => src.Messages.FirstOrDefault() != null ? src.Messages.FirstOrDefault()!.SenderActorId : null)
            .Map(dest => dest.LastUserName, src => src.Messages.FirstOrDefault() != null 
                ? src.Messages.FirstOrDefault()!.SenderUser != null
                    ? 
                    src.Messages.FirstOrDefault()!.SenderUser!.Nickname.IsNotNullOrEmpty()
                        ? src.Messages.FirstOrDefault()!.SenderUser!.Nickname
                        : src.Messages.FirstOrDefault()!.SenderUser!.UserName
                    : string.Empty
                : string.Empty)
            .Map(dest => dest.LastUserAvatarUrl, src => string.Empty)
            .Map(dest => dest.LastMessageId, src => src.Messages.FirstOrDefault()!.PublicId)
            .Map(dest => dest.LastMessageText, src =>
                src.Messages.FirstOrDefault() != null ? src.Messages.FirstOrDefault()!.Kind == MessageKind.Text ? src.Messages.FirstOrDefault()!.Text : "[Attachment]" : null)
            .Map(dest => dest.LastMessageKind, src => src.Messages.FirstOrDefault()!.Kind);
        
        cfg.NewConfig<ConversationItemDto, ConversationDto>()
            .Map(dest => dest.ConversationId, src => src.Id)
            .Map(dest => dest.CustomerAvatarUrl, src => string.Empty)
            .Map(dest => dest.LastSenderRole, src => src.LastMessage != null ? src.LastMessage.SenderRole : null)
            .Map(dest => dest.LastUserId, src => src.LastMessage != null ? src.LastMessage.SenderActorId : null)
            .Map(dest => dest.LastUserName, src => src.LastMessage != null ? src.LastMessage.SenderName : string.Empty)
            .Map(dest => dest.LastUserAvatarUrl, src => string.Empty)
            .Map(dest => dest.LastMessageId, src => src.LastMessage != null ? src.LastMessage.PublicId : null)
            .Map(dest => dest.LastMessageIndex, src => src.LastMessage!.Id)
            .Map(dest => dest.LastMessageText, src => 
                src.LastMessage != null ? src.LastMessage.Kind == MessageKind.Text ? src.LastMessage.Text : "[Attachment]" : null)
            .Map(dest => dest.LastMessageKind, src => src.LastMessage != null ? src.LastMessage.Kind : MessageKind.Text);
        
        cfg.NewConfig<Conversation, ConversationSignalDto>()
            .Map(dest => dest.ConversationId, src => src.PublicId)
            .Map(dest => dest.CustomerAvatarUrl, src => string.Empty)
            .Map(dest => dest.LastSenderRole, src => src.Messages.FirstOrDefault() != null ? (RoleInConversation?)src.Messages.FirstOrDefault()!.SenderRole : null)
            .Map(dest => dest.LastUserId, src => src.Messages.FirstOrDefault() != null ? src.Messages.FirstOrDefault()!.SenderActorId : null)
            .Map(dest => dest.LastUserName, src => src.Messages.FirstOrDefault() != null 
                ? src.Messages.FirstOrDefault()!.SenderUser != null
                    ? 
                    src.Messages.FirstOrDefault()!.SenderUser!.Nickname.IsNotNullOrEmpty()
                        ? src.Messages.FirstOrDefault()!.SenderUser!.Nickname
                        : src.Messages.FirstOrDefault()!.SenderUser!.UserName
                    : string.Empty
                : string.Empty)
            .Map(dest => dest.LastUserAvatarUrl, src => string.Empty)
            .Map(dest => dest.LastMessageId, src => src.Messages.FirstOrDefault() != null ? (Guid?)src.Messages.FirstOrDefault()!.PublicId : null)
            .Map(dest => dest.LastMessageText, src =>
                src.Messages.FirstOrDefault() != null ? src.Messages.FirstOrDefault()!.Kind == MessageKind.Text ? src.Messages.FirstOrDefault()!.Text : "[Attachment]" : null)
            .Map(dest => dest.LastMessageKind, src => src.Messages.FirstOrDefault() != null ? src.Messages.FirstOrDefault()!.Kind.ToString() : null);
        
        cfg.NewConfig<Message, MessageDto>()
            .Map(dest => dest.ConversationId, src => src.Conversation.PublicId)
            .Map(dest => dest.ReplyToMessageId, src => src.ReplyToMessage!.PublicId)
            .Map(dest => dest.DirectMentions, src => src.Attachments.Adapt<List<DirectMention>>())
            .Map(dest => dest.Attachments, src => src.Attachments.Adapt<List<MessageAttachmentDto>>());
  
        cfg.NewConfig<MessageDto, ListedMessageDto>()
            .Map(dest => dest.Id, src => src.PublicId)
            .Map(dest => dest.Index, src => src.Id)
            .Map(dest => dest.SenderUserNickname, src => src.SenderUser!.Nickname);
        
        cfg.NewConfig<MessageAttachment, MessageAttachmentDto>()
            .Map(dest => dest.Attachment, src => src.MediaFile);
    }
}