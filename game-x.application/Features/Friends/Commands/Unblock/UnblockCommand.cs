namespace game_x.application.Features.Friends.Commands.Unblock;

public sealed record UnblockCommand(string TargetUserId) : IRequest<Unit>;