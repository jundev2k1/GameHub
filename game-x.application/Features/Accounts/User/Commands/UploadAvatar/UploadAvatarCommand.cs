using game_x.application.Common.Files;

namespace game_x.application.Features.Accounts.User.Commands.UploadAvatar;

public sealed record UploadAvatarCommand(FileUpload File) : IRequest<string>;