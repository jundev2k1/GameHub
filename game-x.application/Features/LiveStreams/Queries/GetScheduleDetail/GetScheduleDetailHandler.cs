namespace game_x.application.Features.LiveStreams.Queries.GetScheduleDetail;

public sealed class GetScheduleDetailHandler : IQueryHandler<GetScheduleDetailQuery, object>
{
    public async Task<object> Handle(GetScheduleDetailQuery request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return 0;
    }
}
