using game_x.application.Contract.Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace game_x.infrastructure.Security;

public class UserAccessor(IHttpContextAccessor httpContextAccessor)
    : IUserAccessor
{
    public string GetUserId()
    {
        return httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("No user found");
    }

    public ClaimsPrincipal GetClaimsPrincipal()
    {
        return httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedAccessException("No active user context");
    }

    public AppRole GetRoles()
    {
        var roles = httpContextAccessor.HttpContext?.User?
            .FindAll(ClaimTypes.Role)
            .Select(r => r.Value)
            .ToList() ?? [];
        return AppRole.Of(roles);
    }
}
