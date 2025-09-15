namespace game_x.application.Features.Friends.Dtos;

public record SocialLinkDto(
    Guid Id,
    SocialLinkKind Kind,
    SocialLinkState State,
    string? RequesterUserId,
    string? RequesterNickname,
    string? RequesterAvatarUrl,
    string? AddresseeUserId,
    string? AddresseeNickname,
    string? AddresseeAvatarUrl,
    string? BlockerUserId,
    string? BlockerNickname,
    string? BlockerAvatarUrl,
    string? BlockedUserId,
    string? BlockedNickname,
    string? BlockedAvatarUrl,
    DateTime? RespondedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt);