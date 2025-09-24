using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.LiveStreams.Schedules.Commands.UpdateScheduleThumbnail;

public sealed class UpdateScheduleThumbnailHandler(
    ILiveStreamRepo liveStreamRepo,
    IMediaFileRepo mediaFileRepo,
    IUserRepo userRepo,
    IUnitOfWork unitOfWork,
    IFileStorageService storageService,
    IFileManagerCacheService fileManagerCache,
    ILiveStreamManagerCacheService liveStreamManager,
    IUserAccessor userAccessor) : ICommandHandler<UpdateScheduleThumbnailCommand, UpdateScheduleThumbnailResult>
{
    public async Task<UpdateScheduleThumbnailResult> Handle(UpdateScheduleThumbnailCommand request, CancellationToken ct = default)
    {
        // Validate and update thumbnail
        LivestreamSchedule? schedule = null;
        await liveStreamRepo.UpdateAsync(request.Id, async stream =>
        {
            if (stream.Status != LiveStreamStatus.Scheduled && stream.Status != LiveStreamStatus.Live)
                throw new ForbiddenException("Only scheduled or live livestream can update thumbnail.");

            var user = await userRepo.GetUserByIdAsync(userAccessor.GetUserId(), ct);
            if (user.IsUser && stream.AssignedId != user.Id)
                throw new ForbiddenException("You are not allowed to update this livestream.");

            await UploadNewThumbnail(stream, request.FileUpload, ct);
            await unitOfWork.SaveChangesAsync(ct);

            schedule = stream;
        }, ct);

        // Refresh cache and get new url
        await fileManagerCache.RefreshImage(schedule!.Thumbnail!, ct: ct);
        var thumbnail = await fileManagerCache.GetImageUrl(schedule!.Thumbnail!, ct: ct);
        RefreshLiveStreamCache(schedule.StreamKey, schedule.Thumbnail!.Id, thumbnail?.Url);

        return new UpdateScheduleThumbnailResult(thumbnail!.FileName, thumbnail.Url);
    }

    private async Task UploadNewThumbnail(LivestreamSchedule stream, FileUpload file, CancellationToken ct = default)
    {
        // Remove old thumbnail
        if (stream.ThumbnailId.HasValue)
            await mediaFileRepo.RemoveAsync(stream.ThumbnailId.Value, ct);

        // Create image information
        var newFileName = DateTime.Now.ToString("yyyyMMdd_hhMMss") + file.Extension;
        var newFile = MediaFile.Create(
            BucketName.LiveStream,
            ObjectName.LiveStreamThumbnail(stream.PublicId, newFileName),
            file.FileName,
            MimeType.Of(file.ContentType),
            (int)file.Length);
        stream.UpdateThumbnail(newFile);

        // Upload to MinIO
        await storageService.UploadFileAsync(
            file.Content,
            newFile.BucketName,
            newFile.ObjectName,
            newFile.MimeType,
            ct);
    }

    private void RefreshLiveStreamCache(string streamKey, int thumbnailId, string? thumbnailUrl)
    {
        var streamInfo = liveStreamManager.GetLiveStreamStatus(streamKey);
        if (streamInfo is null) return;

        streamInfo.ThumbnailId = thumbnailId;
        streamInfo.Thumbnail = thumbnailUrl;
    }
}
