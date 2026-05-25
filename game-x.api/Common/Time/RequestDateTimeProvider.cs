using game_x.application.Common.Abstractions.Time;

namespace game_x.api.Common.Time;

public sealed class RequestDateTimeProvider(
    IHttpContextAccessor httpContextAccessor,
    IWebHostEnvironment env) : IDateTimeProvider
{
    public DateTime UtcNow
    {
        get
        {
            if (!env.IsDevelopment())
                return DateTime.UtcNow;

            var value = httpContextAccessor
                .HttpContext?
                .Request
                .Headers["X-Debug-Date"]
                .ToString();

            if (DateTime.TryParse(value, out var dt))
                return DateTime.SpecifyKind(dt, DateTimeKind.Utc);

            return DateTime.UtcNow;
        }
    }
}