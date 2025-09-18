namespace game_x.application.Features.Friends.Dtos;

public record FriendSearchResultDto(
    string UserId,
    string Nickname, 
    string? Email, 
    string? AvatarUrl,
    Guid? LinkId,
    SocialLinkState? State,
    string? RequesterUserId,
    string? AddresseeUserId,
    DateTime? LinkCreatedAt);