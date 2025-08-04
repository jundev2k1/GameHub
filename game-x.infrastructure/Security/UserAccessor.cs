using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Exceptions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace game_x.infrastructure.Security;

public sealed class UserAccessor(IHttpContextAccessor httpContextAccessor)
    : IUserAccessor, IServices
{
    public string GetUserId()
    {
        return httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedException("No user found");
    }

    public ClaimsPrincipal GetClaimsPrincipal()
    {
        return httpContextAccessor.HttpContext?.User
            ?? throw new UnauthorizedException("No active user context");
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
