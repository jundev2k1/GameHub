namespace game_x.application.Features.LiveStreams.Queries.JoinLiveStream;

public sealed class JoinLiveStreamHandler : IQueryHandler<JoinLiveStreamQuery, object>
{
    public async Task<object> Handle(JoinLiveStreamQuery request, CancellationToken ct = default)
    {
        await Task.CompletedTask;
        return 0;
    }
}
