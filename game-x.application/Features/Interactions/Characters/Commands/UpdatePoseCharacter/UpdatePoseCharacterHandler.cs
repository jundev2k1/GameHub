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
                // Update character info
                character.Update(
                    request.Name,
                    request.Description,
                    request.Notes);

                // Only update the pose file if there's a new file provided
                if (request.File is not null)
                {
                    // Remove old file if any
                    var targetPose = character.Poses.FirstOrDefault(p => p.PublicId == request.PoseId)
                        ?? throw new NotFoundException(nameof(request.PoseId), request.PoseId!);
                    await mediaFileRepo.RemoveAsync(targetPose.PoseId, ct);

                    // Create new file
                    var poseFile = await CreateNewDefaultPose(character.PublicId, request.File, ct);
                    character.SetPose(request.PoseId!.Value, poseFile);
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
