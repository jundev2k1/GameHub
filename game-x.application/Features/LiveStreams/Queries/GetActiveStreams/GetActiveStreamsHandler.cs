namespace game_x.application.Features.LiveStreams.Queries.GetActiveStreams;

public sealed class GetActiveStreamsHandler : IQueryHandler<GetActiveStreamsQuery, object>
{
    public async Task<object> Handle(GetActiveStreamsQuery request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return 0;
    }
}
