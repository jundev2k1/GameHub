using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGiftIcon;

public sealed class UpdateLiveStreamGiftIconHandler(
    ILiveStreamGiftRepo liveStreamGiftRepo,
    IMediaFileRepo mediaFileRepo,
    IUnitOfWork unitOfWork,
    IFileStorageService storageService,
    IFileManagerCacheService fileManagerCache) : ICommandHandler<UpdateLiveStreamGiftIconCommand, UpdateLiveStreamGiftIconResult>
{
    public async Task<UpdateLiveStreamGiftIconResult> Handle(UpdateLiveStreamGiftIconCommand request, CancellationToken ct = default)
    {
        // Validate and update thumbnail
        await liveStreamGiftRepo.UpdateAsync(request.Id, async gift =>
        {
            await UploadNewIcon(gift, request.FileUpload, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        // Refresh cache and get new url
        var giftAfterUpdate = await liveStreamGiftRepo.GetByIdAsync(request.Id, ct);
        await fileManagerCache.RefreshImage(giftAfterUpdate!.Icon!, ct: ct);
        var thumbnail = await fileManagerCache.GetImageUrl(giftAfterUpdate!.Icon!, ct: ct);

        return new UpdateLiveStreamGiftIconResult(thumbnail!.FileName, thumbnail.Url);
    }

    private async Task UploadNewIcon(LiveStreamGift gift, FileUpload file, CancellationToken ct = default)
    {
        // Remove old thumbnail
        if (gift.IconId.HasValue)
            await mediaFileRepo.RemoveAsync(gift.IconId.Value, ct);

        // Create image information
        var newFileName = DateTime.Now.ToString("yyyyMMdd_hhMMss") + file.Extension;
        var newFile = MediaFile.Create(
            BucketName.LiveStream,
            ObjectName.LiveStreamGiftIcon(gift.PublicId, newFileName),
            file.FileName,
            MimeType.Of(file.ContentType),
            (int)file.Length);
        gift.UpdateIcon(newFile);

        // Upload to MinIO
        await storageService.UploadFileAsync(
            file.Content,
            newFile.BucketName,
            newFile.ObjectName,
            newFile.MimeType,
            ct);
    }
}
