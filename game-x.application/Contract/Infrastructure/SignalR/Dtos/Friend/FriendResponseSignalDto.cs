namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Friend;

public record FriendResponseSignalDto(
    Guid Id,
    SocialLinkState State,
    string? AddresseeUserId,
    string? AddresseeNickname,
    string? AddresseeAvatarUrl,
    string? RequesterUserId);