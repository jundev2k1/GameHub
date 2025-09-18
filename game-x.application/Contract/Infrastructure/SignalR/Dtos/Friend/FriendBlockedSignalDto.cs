namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;

public record FriendBlockedSignalDto(
    Guid LinkId,
    string? BlockerUserId,
    string? BlockerNickname,
    string? BlockerAvatarUrl,
    string? BlockedUserId);