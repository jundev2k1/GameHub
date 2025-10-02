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
            .Map(dest => dest.GuestId, src => src.GuestId ?? String.Empty)
            .Map(dest => dest.CustomerId, src => src.CustomerId ?? String.Empty)
            .Map(dest => dest.CustomerDisplayName, src => src.Customer!.Nickname)
            .Map(dest => dest.CustomerAvatarUrl, src => string.Empty)
            .Map(dest => dest.LastSenderRole, src => src.Messages.FirstOrDefault()!.SenderRole)
            .Map(dest => dest.LastUserId, src => src.Messages.FirstOrDefault()!.SenderActorId)
            .Map(dest => dest.LastUserName, src => 
                src.Messages.FirstOrDefault()!.SenderUser != null 
                    ? src.Messages.FirstOrDefault()!.SenderUser!.Nickname.IsNotNullOrEmpty() 
                        ? src.Messages.FirstOrDefault()!.SenderUser!.Nickname
                        : src.Messages.FirstOrDefault()!.SenderUser!.UserName ?? String.Empty
                    : string.Empty)
            .Map(dest => dest.LastUserName, src =>  src.Messages.FirstOrDefault()!.SenderUser)
            .Map(dest => dest.LastUserAvatarUrl, src => string.Empty)
            .Map(dest => dest.LastMessageAt, src => src.LastMessageAt)
            .Map(dest => dest.LastMessageId, src => src.Messages.FirstOrDefault()!.PublicId)
            .Map(dest => dest.LastMessageText, src => src.Messages.FirstOrDefault()!.Text)
            .Map(dest => dest.LastMessageKind, src => src.Messages.FirstOrDefault()!.Kind);
        
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