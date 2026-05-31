using game_x.domain.ValueObjects;
using game_x.share.Context;

namespace game_x.api.Middleware;

public sealed class AuditSourceMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        using (AuditSourceContext.Use(AuditSource.WebApi))
        {
            await next(context);
        }
    }
}
