using game_x.application.Features.LiveStreams.Streaming.Dtos;

namespace game_x.application.Features.LiveStreams.Streaming.Queries.GetViewersByStream;

public record GetViewersByStreamQuery(string StreamKey) : IQuery<LiveStreamViewerInfoDto[]>;
