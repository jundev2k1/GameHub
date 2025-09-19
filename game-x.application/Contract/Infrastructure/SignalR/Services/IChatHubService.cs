using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;

namespace game_x.application.Contract.Infrastructure.SignalR.Services;

public interface IChatHubService
{
    Task SendSupportMessageAsync(CreatedMessageSignalResult res);
    Task SendDirectMessageAsync(CreatedMessageSignalResult res, string[] memberIds);
    Task SendFriendRequestAsync(FriendRequestSignalDto dto);
    Task SendFriendResponseAsync(FriendResponseSignalDto dto);
    Task SendUnfriendAsync(UnfriendSignalDto dto);
    Task SendFriendBlockedAsync(FriendBlockedSignalDto dto);
    Task SendFriendUnblockedAsync(FriendBlockedSignalDto dto);
}