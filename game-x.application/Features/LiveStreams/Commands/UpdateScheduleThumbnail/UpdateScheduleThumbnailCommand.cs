using game_x.application.Common.Files;

namespace game_x.application.Features.LiveStreams.Commands.UpdateScheduleThumbnail;

public record UpdateScheduleThumbnailCommand(Guid Id, FileUpload FileUpload) : ICommand<UpdateScheduleThumbnailResult>;

public record UpdateScheduleThumbnailResult(string FileName, string Url);
