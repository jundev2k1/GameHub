namespace game_x.application.Features.Friends.Commands.Unfriend;

public sealed record UnfriendCommand(string TargetUserId) : IRequest<Unit>;