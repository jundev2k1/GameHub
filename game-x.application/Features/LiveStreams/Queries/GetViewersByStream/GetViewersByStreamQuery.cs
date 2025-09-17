using game_x.application.Features.LiveStreams.Dtos;

namespace game_x.application.Features.LiveStreams.Queries.GetViewersByStream;

public record GetViewersByStreamQuery(string StreamKey) : IQuery<LiveStreamViewerInfoDto[]>;
