namespace game_x.application.Features.Friends.Dtos;

public record FriendRequestDto(
    Guid Id,
    SocialLinkKind Kind,
    SocialLinkState State,
    string? RequesterUserId,
    string? RequesterNickname,
    string? RequesterAvatarUrl,
    DateTime? RespondedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt);