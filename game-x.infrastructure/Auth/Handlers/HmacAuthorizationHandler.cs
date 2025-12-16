using game_x.application.Contract.Infrastructure.Security;
using game_x.persistence.Requirements;
using game_x.share.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace game_x.infrastructure.Auth.Handlers;

public sealed class HmacAuthorizationHandler(
    IHttpContextAccessor httpContextAccessor,
    IHmacValidator hmacValidator,
    IOptions<HmacSettings> options,
    ILogger<HmacAuthorizationHandler> logger) : AuthorizationHandler<HmacRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HmacRequirement requirement)
    {
        var httpContext = httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            logger.LogWarning("HMAC validation failed: HttpContext is null.");
            return;
        }

        var header = httpContext.Request.Headers;
        if (!header.ContainsKey(options.Value.NonceHeader)
            || !header.ContainsKey(options.Value.SignatureHeader)
            || !header.ContainsKey(options.Value.TimestampHeader))
        {
            logger.LogWarning("HMAC validation failed: Missing X-Key header.");
            return;
        }

        var isValid = await hmacValidator.ValidateAsync(httpContext.Request);
        if (!isValid)
        {

            logger.LogWarning("HMAC validation failed: Invalid request.");
            return;
        }

        context.Succeed(requirement);
    }
}
