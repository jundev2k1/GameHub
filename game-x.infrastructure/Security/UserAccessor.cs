using game_x.application.Common.Abstractions;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Exceptions;
using game_x.share.Extensions;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace game_x.infrastructure.Security;

public sealed class UserAccessor(
    IHttpContextAccessor httpContextAccessor,
    IJwtTokenGenerator tokenGenerator) : IUserAccessor, IServices
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

    public JwtPayloadDto GetTokenInfo()
    {
        var token = httpContextAccessor.HttpContext?.Request.Headers.Authorization.ToStringOrEmpty()
            ?? throw new UnauthorizedException("Not exist token.");

        var rawToken = token.Split(" ")[1];
        return tokenGenerator.DecodeToken(rawToken)
            ?? throw new UnauthorizedException("Token is invalid.");
    }

    public AppRole GetRoles()
    {
        var roles = httpContextAccessor.HttpContext?.User?
            .FindAll(ClaimTypes.Role)
            .Select(r => r.Value)
            .ToList() ?? [];
        return AppRole.Of(roles);
    }

    public string GetIpAddress()
    {
        var ip = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
        if (string.IsNullOrWhiteSpace(ip))
            ip = httpContextAccessor.HttpContext?.Request?.Headers["X-Forwarded-For"].FirstOrDefault();
        return ip ?? "unknown";
    }

    public string GetUserAgent()
    {
        return httpContextAccessor.HttpContext?.Request?.Headers.UserAgent.ToString() ?? "unknown";
    }

    public string GetDeviceInfo()
    {
        var userAgent = GetUserAgent();
        if (userAgent.Contains("Mobile", StringComparison.OrdinalIgnoreCase)) return "Mobile";
        if (userAgent.Contains("Windows", StringComparison.OrdinalIgnoreCase)) return "Windows PC";
        if (userAgent.Contains("Macintosh", StringComparison.OrdinalIgnoreCase)) return "MacOS";
        return "Other";
    }
}
