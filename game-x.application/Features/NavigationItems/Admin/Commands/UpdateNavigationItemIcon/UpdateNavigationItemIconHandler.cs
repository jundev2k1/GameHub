using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Repo;
using Microsoft.EntityFrameworkCore;

namespace game_x.application.Features.NavigationItems.Admin.Commands.UpdateNavigationItemIcon;

public sealed class UpdateNavigationItemIconHandler(
    IUnitOfWork unitOfWork,
    INavigationItemRepo navigationItemRepo,
    IFileStorageService fileStorage,
    IFileManagerCacheService fileManagerCache,
    INavigationCacheService navigationCache) : ICommandHandler<UpdateNavigationItemIconCommand, string>
{
    public async Task<string> Handle(UpdateNavigationItemIconCommand request, CancellationToken ct = default)
    {
        MediaFile? file = null;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            file = await UploadFiles(request.Id, request.File, ct);
            await navigationItemRepo.UpdateAsync(
                request.Id,
                item => item.UploadIcon(file),
                query => query.Include(q => q.Icon),
                ct);
        }, ct);

        // Refresh cache
        await fileManagerCache.RefreshImage(file!, ct: ct);

        // Refreshing navigation items cache
        await navigationCache.RefreshNavigationItemsAsync(ct);

        // Get url
        var avatarUrl = await fileManagerCache.GetFileUrl(file, ct);
        return avatarUrl ?? string.Empty;
    }

    private async Task<MediaFile> UploadFiles(Guid itemId, FileUpload file, CancellationToken ct)
    {
        var newFileName = Guid.CreateVersion7() + file.Extension;
        var newFile = MediaFile.Create(
            bucketName: BucketName.Navigation,
            objectName: ObjectName.NavigationItem(itemId, newFileName),
            fileName: file.FileName,
            mimeType: MimeType.Of(file.ContentType),
            sizeBytes: Convert.ToInt32(file.Length));
        await fileStorage.UploadFileAsync(file.Content, newFile.BucketName, newFile.ObjectName, newFile.MimeType, ct);
        return newFile;
    }
}
