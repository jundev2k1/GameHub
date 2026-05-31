using game_x.application.Common.Files;

namespace game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGiftIcon;

public record UpdateLiveStreamGiftIconCommand(Guid Id, FileUpload FileUpload) : ICommand<UpdateLiveStreamGiftIconResult>;

public record UpdateLiveStreamGiftIconResult(string FileName, string Url);
