namespace game_x.infrastructure.SignalR.Groups;

/// <summary>Chat-specific groups – only ChatHub should reference this</summary>
internal static class ChatGroups
{
    public static string BackOffice() => "chat:back-office";
    public static string Public => "chat:public:conv";
    /// <summary>Get upserted inbox</summary>
    public static string PublicIdle => "chat:public:idle";
    public static string OnlineAll => "chat:online";
    public static string IdleAgent => "chat:idle-agent";
    /// <summary>Agent Inbox screen</summary>
    public static string AgentInbox => "chat:agent-inbox";
    public static string IdleMember(string userId) => $"chat:idle-member:{userId}";
    /// <summary>Inbox screen</summary>
    public static string MemberInbox(string userId) => $"chat:member-inbox:{userId}";
    public static string Conversation(Guid conversationId) => $"chat:conv:{conversationId.ToString()}";
}