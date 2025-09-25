using game_x.application.Common.Files;

namespace game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGiftAnimation;

public record UpdateLiveStreamGiftAnimationCommand(Guid Id, FileUpload FileUpload) : ICommand<UpdateLiveStreamGiftAnimationResult>;

public record UpdateLiveStreamGiftAnimationResult(string FileName, string Url);
