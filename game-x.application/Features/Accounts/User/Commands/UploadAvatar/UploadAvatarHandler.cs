using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Accounts.User.Commands.UploadAvatar;

public sealed class UploadAvatarHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IFileStorageService fileStorage,
    IFileManagerCacheService fileManagerCache) : ICommandHandler<UploadAvatarCommand, string>
{
    public async Task<string> Handle(UploadAvatarCommand request, CancellationToken ct)
    {
        var userId = userAccessor.GetUserId();
        var mediaFile = await UploadFiles(userId, request.File, ct);

        // Update user avatar
        await userRepo.UpdateAsync(userId, targetUser =>
        {
            targetUser.Avatar = mediaFile;
        }, ct);
        await unitOfWork.SaveChangesAsync(ct);

        // Refresh cache
        await fileManagerCache.RefreshImage(mediaFile, ct: ct);

        // Get url
        var avatarUrl = await fileManagerCache.GetFileUrl(mediaFile, ct);
        return avatarUrl ?? string.Empty;
    }

    private async Task<MediaFile> UploadFiles(string userId, FileUpload file, CancellationToken ct)
    {
        var newFileName = Guid.NewGuid() + file.Extension;
        var newFile = MediaFile.Create(
            bucketName: BucketName.User,
            objectName: ObjectName.Avatar(userId, newFileName),
            fileName: file.FileName,
            mimeType: MimeType.Of(file.ContentType),
            sizeBytes: Convert.ToInt32(file.Length));
        await fileStorage.UploadFileAsync(file.Content, newFile.BucketName, newFile.ObjectName, newFile.MimeType, ct);
        return newFile;
    }
}
