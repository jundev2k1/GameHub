using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Repo;
using game_x.share.Extensions;
using Microsoft.EntityFrameworkCore;

namespace game_x.application.Features.Games.Admin.Commands.UploadGameMediaSource;

public sealed class UploadGameMediaSourceHandler(
    IUnitOfWork unitOfWork,
    IGameMediaRepo gameMediaRepo,
    IMediaFileRepo mediaFileRepo,
    IFileStorageService fileStorage,
    IFileManagerCacheService fileManagerCache,
    IGameProviderCacheService gameProviderCache) : ICommandHandler<UploadGameMediaSourceCommand, UploadGameMediaSourceResult>
{
    public async Task<UploadGameMediaSourceResult> Handle(UploadGameMediaSourceCommand request, CancellationToken ct = default)
    {
        // Validate target media first
        var targetMedia = await ValidateTargetMediaAsync(request.GameId, request.MediaId, ct);

        // Upload file to object storage first
        var uploadedVideo = await HandleUploadVideoAsync(
            request.GameId,
            request.MediaId,
            request.File,
            ct);

        try
        {
            // Persist database changes in short transaction
            await PersistChangesAsync(targetMedia, uploadedVideo, ct);

            // Refresh cache
            await RefreshCachesAsync(request.GameId, request.MediaId, uploadedVideo, ct);
        }
        catch (Exception ex)
        {
            // Cleanup orphan object if db failed
            this.FileDeleted = uploadedVideo;
            this.Ex = ex;
        }
        finally
        {
            // Cleanup orphan object if db failed
            await CleanupUploadedFileAsync(ct);
        }

        // Throw if there are any errors
        if (this.Ex != null) throw this.Ex;

        var url = await fileManagerCache.GetFileUrl(uploadedVideo, ct);
        return new UploadGameMediaSourceResult(url ?? string.Empty);
    }

    private async Task<GameMedia> ValidateTargetMediaAsync(Guid gameId, Guid mediaId, CancellationToken ct)
    {
        var targetMedia = await gameMediaRepo.GetAsync(mediaId, ct);
        if (targetMedia.Game.PublicId != gameId)
            throw new BadRequestException(MessageCode.System.ResourceNotFound, new { gameId = targetMedia.Game.PublicId });

        if ((targetMedia.Type is not GameMediaType.Video)
            || (targetMedia.Category is not GameMediaCategory.TutorialVideo and not GameMediaCategory.DemoVideo))
            throw new BadRequestException(
                MessageCode.System.InvalidResourceState,
                new
                {
                    type = targetMedia.Type.ToCamelCase(),
                    category = targetMedia.Category.ToCamelCase()
                });

        return targetMedia;
    }

    private async Task<MediaFile> HandleUploadVideoAsync(Guid gameId, Guid gameMediaId, FileUpload fileUpload, CancellationToken ct)
    {
        var objectName = ObjectName.GameMedia(
            gameId,
            gameMediaId,
            fileName: $"{Guid.CreateVersion7()}{fileUpload.Extension}");

        var mediaFile = MediaFile.Create(
            BucketName.Game,
            objectName,
            fileUpload.FileName,
            MimeType.Of(fileUpload.ContentType),
            Convert.ToInt32(fileUpload.Length));

        await fileStorage.UploadFileAsync(
            fileUpload.Content,
            fileUpload.Length,
            mediaFile.BucketName,
            mediaFile.ObjectName,
            mediaFile.MimeType,
            ct);

        return mediaFile;
    }

    private async Task PersistChangesAsync(GameMedia targetMedia, MediaFile uploadedVideo, CancellationToken ct)
    {
        await unitOfWork.WithTransactionAsync(async () =>
        {
            // Handle creating new mediafile
            await mediaFileRepo.AddAsync(uploadedVideo, ct);
            await unitOfWork.SaveChangesAsync(ct);

            await gameMediaRepo.UpdateAsync(
                targetMedia.PublicId,
                query => query.Include(q => q.File),
                async (gameMedia) =>
                {
                    // Remove old file metadata
                    if (gameMedia.FileId.HasValue)
                    {
                        this.FileDeleted = gameMedia.File;
                        await mediaFileRepo.RemoveAsync(gameMedia.FileId.Value, ct);
                    }

                    // Update media source
                    gameMedia.UpdateFile(uploadedVideo.Id);
                },
                ct);
        }, ct);
    }

    private async Task CleanupUploadedFileAsync(CancellationToken ct)
    {
        if (this.FileDeleted is null) return;

        await fileStorage.DeleteFileAsync(this.FileDeleted.BucketName, this.FileDeleted.ObjectName, ct);
    }

    private async Task RefreshCachesAsync(Guid gameId, Guid gameMediaId, MediaFile file, CancellationToken ct)
    {
        await fileManagerCache.RefreshImage(file, ct: ct);

        // Handle refresh game media from in-memory cache
        await gameProviderCache.RefreshSpecifyGameMediaAsync(gameId, gameMediaId, ct);
    }

    private MediaFile? FileDeleted { get; set; }
    private Exception? Ex { get; set; }
}
