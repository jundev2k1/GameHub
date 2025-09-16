namespace game_x.application.Features.Friends.Dtos;

public record IncomingFriendRequestDto(
    Guid Id,
    string? RequesterUserId,
    string? RequesterNickname,
    string? RequesterAvatarUrl,
    DateTime CreatedAt,
    DateTime UpdatedAt);