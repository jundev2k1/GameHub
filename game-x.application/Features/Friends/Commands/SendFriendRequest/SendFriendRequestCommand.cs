namespace game_x.application.Features.Friends.Commands.SendFriendRequest;

public sealed record SendFriendRequestCommand(string TargetUserId) : IRequest<Unit>;