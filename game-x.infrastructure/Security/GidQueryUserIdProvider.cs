using System.Security.Claims;
using Microsoft.AspNetCore.SignalR;

namespace game_x.infrastructure.Security;

public class GidQueryUserIdProvider(): IUserIdProvider
{
    public string? GetUserId(HubConnectionContext conn)
    {
        var sub = conn.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrWhiteSpace(sub)) return sub;
        
        var gid = conn.GetHttpContext()?.Request.Query["gid"].ToString();
        if (string.IsNullOrWhiteSpace(gid)) return null;
        if (!Guid.TryParse(gid, out var g)) return null;

        return $"{g:D}";
    }
}