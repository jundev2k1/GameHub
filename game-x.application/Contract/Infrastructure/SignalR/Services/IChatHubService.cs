using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;
using game_x.application.Features.Chat.Dtos;

namespace game_x.application.Contract.Infrastructure.SignalR.Services;

public interface IChatHubService
{
    Task SendSupportConversationUnreadAsync(IReadOnlyCollection<ConversationUnreadDto> dto);
    Task SendSupportConversationClaimedAsync(ConversationSignalDto dto);
    Task SendDeletedMessageAsync(DeletedMessageDto dto);
    Task SendMarkAsReadAsync(ConversationSignalDto dto, string userId, AppRole role);
    Task SendPublicMessageAsync(CreatedMessageSignalResult res);
    Task SendSupportMessageAsync(CreatedMessageSignalResult res);
    Task SendSupportMessageV2Async(CreatedMessageSignalResult res);
    Task SendDirectMessageAsync(CreatedMessageSignalResult res, ConvMemberDto[] members);
    Task SendFriendRequestAsync(FriendRequestSignalDto dto);
    Task SendFriendResponseAsync(FriendResponseSignalDto dto);
    Task SendUnfriendAsync(UnfriendSignalDto dto);
    Task SendFriendBlockedAsync(FriendBlockedSignalDto dto);
    Task SendFriendUnblockedAsync(FriendBlockedSignalDto dto);
}