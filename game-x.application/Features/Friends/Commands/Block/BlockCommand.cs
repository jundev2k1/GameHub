namespace game_x.application.Features.Friends.Commands.Block;

public sealed record BlockCommand(string TargetUserId) : IRequest<Unit>;