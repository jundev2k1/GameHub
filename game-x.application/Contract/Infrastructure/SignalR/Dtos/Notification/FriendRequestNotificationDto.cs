namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;

public sealed class FriendRequestNotificationDto
{
    public Guid Id { get; set; }
    public string? AddresseeUserId { get; set; }
    public string? RequesterUserId { get; set; }
    public string? RequesterNickname { get; set; }
    public string? RequesterAvatarUrl { get; set; }
}