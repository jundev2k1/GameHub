using game_x.application.Contract.Infrastructure.Caching;

namespace game_x.application.Events.Account.OnInvalidUserChanged;

public sealed class OnInvalidUserChangedHandler(IUserCacheService userCache)
    : IApplicationEventHandler<OnInvalidUserChangedEvent>
{
    public async Task Handle(OnInvalidUserChangedEvent @event, CancellationToken ct = default)
    {
        await userCache.RefreshInactiveUser(ct);
    }
}
