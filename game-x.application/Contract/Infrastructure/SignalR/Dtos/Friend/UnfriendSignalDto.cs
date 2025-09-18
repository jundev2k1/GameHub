namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;

public record UnfriendSignalDto(
    Guid LinkId,
    string? UnfrienderId,
    string? UnfrienderNickname,
    string? UnfrienderAvatarUrl,
    string? UnfriendedUserId);