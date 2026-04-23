using game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;

namespace game_x.application.Events.Friendship.OnUnfriend;

public record OnUnfriendEvent(UnfriendSignalDto Dto) : IApplicationEvent;