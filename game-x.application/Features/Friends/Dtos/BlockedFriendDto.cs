namespace game_x.application.Features.Friends.Dtos;

public record BlockedFriendDto(
    Guid LinkId,
    string? BlockedUserId,
    string? BlockedNickname,
    string? BlockedAvatarUrl,
    DateTime? RespondedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt);