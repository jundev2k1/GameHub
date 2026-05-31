using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Interactions.Characters.Commands.UpdateDefaultPoseCharacter;

public sealed class UpdateDefaultPoseCharacterHandler(
    IUnitOfWork unitOfWork,
    IInteractionCharacterRepo characterRepo,
    IMediaFileRepo mediaFileRepo,
    IFileStorageService fileStorage,
    IFileManagerCacheService fileManagerCache) : ICommandHandler<UpdateDefaultPoseCharacterCommand>
{
    public async Task<Unit> Handle(UpdateDefaultPoseCharacterCommand request, CancellationToken ct = default)
    {
        MediaFile? fileAfterUpdated = null;
        await unitOfWork.WithTransactionAsync(async () =>
        {
            await characterRepo.UpdateAsync(request.Id!.Value, async character =>
            {
                var oldPoseId = character.DefaultPoseId;

                // Create new file
                var poseFile = await CreateNewDefaultPose(character.PublicId, request.File, ct);
                character.SetDefaultPose(poseFile);

                // Remove old file if any
                await mediaFileRepo.RemoveAsync(oldPoseId, ct);

                // Save new file info to db for caching purpose
                fileAfterUpdated = character.DefaultPose;
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
