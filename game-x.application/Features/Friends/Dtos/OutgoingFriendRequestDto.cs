namespace game_x.application.Features.Friends.Dtos;

public record OutgoingFriendRequestDto(
    Guid LinkId,
    string? AddresseeUserId,
    string? AddresseeNickname,
    string? AddresseeAvatarUrl,
    DateTime CreatedAt,
    DateTime UpdatedAt);