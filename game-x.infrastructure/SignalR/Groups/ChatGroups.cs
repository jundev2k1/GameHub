namespace game_x.infrastructure.SignalR.Groups;

/// <summary>Chat-specific groups – only ChatHub should reference this</summary>
internal static class ChatGroups
{
    private const string Prefix = "chat";
    public static string BackOffice() => $"{Prefix}:back-office";
    public static string Public => $"{Prefix}:public:conv";
    /// <summary>Get upserted inbox</summary>
    public static string PublicIdle => $"{Prefix}:public:idle";
    public static string OnlineAll => $"{Prefix}:online";
    public static string IdleAgent => $"{Prefix}:idle-agent";
    /// <summary>Agent Inbox screen</summary>
    public static string AgentInbox => $"{Prefix}:agent-inbox";
    public static string IdleMember(string userId) => $"{Prefix}:idle-member:{userId}";
    /// <summary>Inbox screen</summary>
    public static string MemberInbox(string userId) => $"{Prefix}:member-inbox:{userId}";
    public static string Conversation(Guid conversationId) => $"{Prefix}:conv:{conversationId.ToString()}";
}