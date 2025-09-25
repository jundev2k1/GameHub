namespace game_x.application.Contract.Infrastructure.SignalR.Dtos.Notification;

public sealed class FriendResponseNotificationDto
{
    public Guid Id { get; set; }
    public SocialLinkState State { get; set; }
    public string? RequesterUserId { get; set; }
    public string? AddresseeUserId { get; set; }
    public string? AddresseeNickname { get; set; }
    public string? AddresseeAvatarUrl { get; set; }
}