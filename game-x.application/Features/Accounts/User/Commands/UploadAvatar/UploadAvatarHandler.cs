using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Infrastructure.Logger;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Accounts.User.Commands.UploadAvatar;

public class UploadAvatarHandler(
    IUnitOfWork unitOfWork,
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IFileStorageService fileStorage,
    IFileManagerCacheService fileManagerCache,
    IAppLogger<domain.Entities.User> logger): IRequestHandler<UploadAvatarCommand, string>
{
    public async Task<string> Handle(UploadAvatarCommand request, CancellationToken ct)
    {
        try
        {
            var userId = userAccessor.GetUserId();
            var mediaFile = await UploadFiles(userId, request.File, ct);
            
            await userRepo.UpdateAsync(userId, targetUser =>
            {
                targetUser.Avatar = mediaFile;
            }, ct);
            await unitOfWork.SaveChangesAsync(ct);
            await fileManagerCache.RefreshImage(mediaFile, ct: ct);
            var avatarInfo = await fileManagerCache.GetFileUrl(mediaFile, ct);

            return avatarInfo?.Url ?? string.Empty;
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            throw new BadRequestException(MessageCode.System.SystemError);
        }
    }
    
    private async Task<MediaFile> UploadFiles(string userId, FileUpload file, CancellationToken ct)
    {
        var newFileName = Guid.NewGuid() + file.Extension;
        var objectName = ObjectName.Avatar(userId, newFileName);
        await fileStorage.UploadFileAsync(file.Content, BucketName.User, objectName, MimeType.Of(file.ContentType), ct);
        return CreateMediaFile(file, objectName);
    }
    
    private static MediaFile CreateMediaFile(FileUpload file, ObjectName objectName)
    {
        return MediaFile.Create(
            bucketName: BucketName.User,
            objectName: objectName,
            fileName: file.FileName,
            mimeType: MimeType.Of(file.ContentType),
            sizeBytes: Convert.ToInt32(file.Length));
    }
}