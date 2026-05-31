namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;

public sealed record FriendRequestSignalDto(
    Guid LinkId,
    string? AddresseeUserId,
    string? RequesterUserId,
    string? RequesterNickname,
    string? RequesterAvatarUrl);