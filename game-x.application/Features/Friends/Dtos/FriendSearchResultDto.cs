namespace game_x.application.Features.Friends.Dtos;

public record FriendSearchResultDto(
    string Id, 
    string Nickname, 
    string UserName, 
    string Email, 
    string? AvatarUrl);
