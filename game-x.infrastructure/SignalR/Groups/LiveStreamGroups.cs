namespace game_x.infrastructure.SignalR.Groups;

public class LiveStreamGroups
{
    public static string Stream(string streamKey) => $"stream-{streamKey}";
    public static string StreamMember(string streamKey, string userId) => $"stream-{streamKey}-member-{userId}";
    public static string StreamHost(string streamKey) => $"stream-{streamKey}-host";
}