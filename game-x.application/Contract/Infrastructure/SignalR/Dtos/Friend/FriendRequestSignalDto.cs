namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;

public record FriendRequestSignalDto(
    Guid Id,
    SocialLinkKind Kind,
    SocialLinkState State,
    string? AddresseeUserId,
    string? RequesterUserId,
    string? RequesterNickname,
    string? RequesterAvatarUrl,
    DateTime? RespondedAt);