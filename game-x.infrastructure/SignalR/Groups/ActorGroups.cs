namespace game_x.infrastructure.SignalR.Groups;

/// <summary>Actor / role-based groups shared across hubs</summary>
internal static class ActorGroups
{
    public static string Member(string userId) => $"member:{userId}";
    public static string Guest(string guestId) => $"guest:{guestId}";
    public static string Admin(string userId) => $"admin:{userId}";
    public static string Cs(string userId) => $"cs:{userId}";
    public static string Broadcast(string roleName) => $"role:{roleName}";
}