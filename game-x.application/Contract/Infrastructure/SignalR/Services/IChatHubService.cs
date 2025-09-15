using game_x.application.Contract.Infrastructure.SignalR.Dtos.Chat;
using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;

namespace game_x.application.Contract.Infrastructure.SignalR.Services;

public interface IChatHubService
{
    Task SendSupportMessageAsync(CreatedMessageSignalResult res);
    Task SendFriendRequestAsync(FriendRequestSignalDto dto);
}