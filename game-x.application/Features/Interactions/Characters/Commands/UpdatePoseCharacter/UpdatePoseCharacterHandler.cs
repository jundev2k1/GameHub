using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Interactions.Characters.Commands.UpdatePoseCharacter;

public sealed class UpdatePoseCharacterHandler(
    IUnitOfWork unitOfWork,
    IInteractionCharacterRepo characterRepo,
    IMediaFileRepo mediaFileRepo,
    IFileStorageService fileStorage,
    IFileManagerCacheService fileManagerCache) : ICommandHandler<UpdatePoseCharacterCommand>
{
    public async Task<Unit> Handle(UpdatePoseCharacterCommand request, CancellationToken ct = default)
    {
        MediaFile? fileAfterUpdated = null;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await characterRepo.UpdateAsync(request.Id!.Value, async character =>
            {
                var targetPose = character.Poses.FirstOrDefault(p => p.PublicId == request.PoseId)
                    ?? throw new NotFoundException(nameof(request.PoseId), request.PoseId!);

                // Update character info
                targetPose.Update(
                    request.Name?.Trim() ?? string.Empty,
                    request.Description?.Trim() ?? string.Empty,
                    request.Notes?.Trim() ?? string.Empty,
                    targetPose.Priority);

                // Only update the pose file if there's a new file provided
                if (request.File is not null)
                {
                    var oldId = targetPose.PoseId;

                    // Create new file
                    var poseFile = await CreateNewDefaultPose(character.PublicId, request.File, ct);
                    targetPose.UpdatePose(poseFile);

                    // Remove old file if any
                    await mediaFileRepo.RemoveAsync(oldId, ct);

                    fileAfterUpdated = poseFile;
                }
            }, ct);
        }, ct);

        if (fileAfterUpdated is not null && fileAfterUpdated.Id != default)
            await fileManagerCache.RefreshImage(fileAfterUpdated, ct: ct);

        return Unit.Value;
    }

    private async Task<MediaFile> CreateNewDefaultPose(Guid characterId, FileUpload file, CancellationToken ct)
    {
        // Create image information
        var newFileName = DateTime.Now.ToString("yyyyMMdd_hhMMss") + file.Extension;
        var newFile = MediaFile.Create(
            BucketName.Interaction,
            ObjectName.InteractionCharacter(characterId, newFileName),
            file.FileName,
            MimeType.Of(file.ContentType),
            (int)file.Length);

        // Upload to MinIO
        await fileStorage.UploadFileAsync(
            file.Content,
            newFile.BucketName,
            newFile.ObjectName,
            newFile.MimeType,
            ct);

        return newFile;
    }
}
