using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Gifts.Commands.UpdateLiveStreamGiftAnimation;

public sealed class UpdateLiveStreamGiftAnimationHandler(
    ILiveStreamGiftRepo liveStreamGiftRepo,
    IMediaFileRepo mediaFileRepo,
    IUnitOfWork unitOfWork,
    IFileStorageService storageService,
    IFileManagerCacheService fileManagerCache,
    ILiveStreamManagerCacheService liveStreamManager) : ICommandHandler<UpdateLiveStreamGiftAnimationCommand, UpdateLiveStreamGiftAnimationResult>
{
    public async Task<UpdateLiveStreamGiftAnimationResult> Handle(UpdateLiveStreamGiftAnimationCommand request, CancellationToken ct = default)
    {
        // Validate and update thumbnail
        await liveStreamGiftRepo.UpdateAsync(request.Id, async gift =>
        {
            await UploadNewAnimation(gift, request.FileUpload, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }, ct);

        // Refresh cache and get new url
        var giftAfterUpdate = await liveStreamGiftRepo.GetByIdAsync(request.Id, ct);
        await fileManagerCache.RefreshImage(giftAfterUpdate!.Icon!, ct: ct);
        var animation = await fileManagerCache.GetFileInfo(giftAfterUpdate!.Icon!, ct: ct);
        await liveStreamManager.RefreshGiftCacheAsync(ct);

        return new UpdateLiveStreamGiftAnimationResult(animation!.FileName, animation.Url);
    }

    private async Task UploadNewAnimation(LiveStreamGift gift, FileUpload file, CancellationToken ct = default)
    {
        // Remove old thumbnail
        if (gift.AnimationId.HasValue)
            await mediaFileRepo.RemoveAsync(gift.AnimationId.Value, ct);

        // Create image information
        var newFileName = DateTime.Now.ToString("yyyyMMdd_hhMMss") + file.Extension;
        var newFile = MediaFile.Create(
            BucketName.LiveStream,
            ObjectName.LiveStreamGiftAnimation(gift.PublicId, newFileName),
            file.FileName,
            MimeType.Of(file.ContentType),
            (int)file.Length);
        gift.UpdateAnimation(newFile);

        // Upload to MinIO
        await storageService.UploadFileAsync(
            file.Content,
            newFile.BucketName,
            newFile.ObjectName,
            newFile.MimeType,
            ct);
    }
}
