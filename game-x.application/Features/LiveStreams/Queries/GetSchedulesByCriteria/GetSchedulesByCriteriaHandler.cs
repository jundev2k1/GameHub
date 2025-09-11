namespace game_x.application.Features.LiveStreams.Queries.GetSchedulesByCriteria;

public sealed class GetSchedulesByCriteriaHandler : IQueryHandler<GetSchedulesByCriteriaQuery, object>
{
    public async Task<object> Handle(GetSchedulesByCriteriaQuery request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return 0;
    }
}
