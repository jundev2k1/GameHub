using System.Text;
using System.Text.RegularExpressions;

namespace game_x.infrastructure.SignalR;

/// <summary>
/// Centralized builder for SignalR group names across all hubs.
/// Format: {prefix}:{area}:{kind}:{id}
/// Example: gx:chat:conv:123, gx:user:42, gx:admin:user:42
/// </summary>
internal static class GroupNames
{
    // App-wide prefix (override via env var if you run multiple apps in same SignalR backplane)
    public static readonly string Prefix =
        Environment.GetEnvironmentVariable("HUB_GROUP_PREFIX") ?? "gx";

    // ---------- Actor groups ----------
    public static string Member(string userId) => Build("member", San(userId));
    public static string Guest(string guestId) => Build("guest", San(guestId));
    public static string Admin(string userId) => Build("admin", San(userId));
    public static string Cs(string userId) => Build("cs", San(userId));
    public static string Role(string roleName) => Build("role", San(roleName)); // e.g., Admin, User

    // ---------- Chat-specific ----------
    public const string Public = "public:conv";
    public const string PublicIdle = "public:idle"; // Get upserted inbox
    public const string OnlineAll = "online";
    public const string IdleAgent = "idle-agent";
    public const string AgentInbox = "agent-inbox"; // is on Agent Inbox screen
    public static string IdleMember(string userId) => Build("idle-member", San(userId));
    public static string MemberInbox(string userId) => Build("member-inbox", San(userId)); // is on Inbox screen
    public static string Conversation(Guid conversationId) => Build("chat", "conv", conversationId.ToString());
    
    // ---------- Helpers ----------

    private static string Build(params string[] parts)
    {
        var sb = new StringBuilder(Prefix);
        foreach (var p in parts)
        {
            if (string.IsNullOrWhiteSpace(p)) continue;
            sb.Append(':').Append(p);
        }
        // SignalR group names are strings; keep them short & ASCII-only if you can.
        return sb.ToString();
    }

    // Normalize segment: lower-case, trim, replace spaces, strip disallowed chars
    private static string San(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
        var s = raw.Trim().ToLowerInvariant().Replace(' ', '-');
        // allow a-z0-9-_.: only inside segments (we won't include ':' in segments anyway)
        s = Regex.Replace(s, @"[^a-z0-9_\-\.]", string.Empty);
        // keep it reasonable; SignalR does not enforce a strict limit, but backplane might
        return s.Length <= 120 ? s : s[..120];
    }
}