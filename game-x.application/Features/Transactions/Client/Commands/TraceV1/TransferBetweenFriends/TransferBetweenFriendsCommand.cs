namespace game_x.application.Features.Transactions.Client.Commands.TraceV1.TransferBetweenFriends;

public sealed record TransferBetweenFriendsCommand(
    decimal Amount,
    string TargetUserId,
    string Note) : ICommand<Unit>;