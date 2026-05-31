using game_x.application.Common.Files;

namespace game_x.application.Features.Games.Admin.Commands.UploadGameMediaSource;

public record UploadGameMediaSourceCommand(
    Guid GameId,
    Guid MediaId,
    FileUpload File) : ICommand<UploadGameMediaSourceResult>;

public record UploadGameMediaSourceResult(string Url);
