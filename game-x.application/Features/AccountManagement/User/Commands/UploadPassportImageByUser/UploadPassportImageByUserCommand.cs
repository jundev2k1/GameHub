using game_x.application.Common.Files;

namespace game_x.application.Features.AccountManagement.User.Commands.UploadPassportImageByUser;

public sealed record UploadPassportImageByUserCommand(string PassportNumber, FileUpload File) : ICommand<UploadPassportImageResult>;

public record UploadPassportImageResult(string Message, string PassportImageUrl);