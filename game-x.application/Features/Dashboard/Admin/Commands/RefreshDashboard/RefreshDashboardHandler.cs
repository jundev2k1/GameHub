using game_x.application.Events.OnDashboardRefresh;

namespace game_x.application.Features.Dashboard.Admin.Commands.RefreshDashboard;

public sealed class RefreshDashboardHandler(IApplicationEventDispatcher eventDispatcher)
    : ICommandHandler<RefreshDashboardCommand>
{
    public async Task<Unit> Handle(RefreshDashboardCommand request, CancellationToken ct = default)
    {
        await eventDispatcher.Publish(new OnDashboardRefreshEvent(), ct);
        return Unit.Value;
    }
}
