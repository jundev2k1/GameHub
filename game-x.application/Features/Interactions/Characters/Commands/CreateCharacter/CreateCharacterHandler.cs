using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.Interactions.Characters.Commands.CreateCharacter;

public sealed class CreateCharacterHandler(
    IUnitOfWork unitOfWork,
    IInteractionCharacterRepo characterRepo,
    IFileStorageService fileStorage,
    IFileManagerCacheService fileManagerCache) : ICommandHandler<CreateCharacterCommand>
{
    public async Task<Unit> Handle(CreateCharacterCommand request, CancellationToken ct = default)
    {
        var newCharacter = InteractionCharacter.Create(
            request.Name.Trim(),
            request.Description.Trim(),
            request.Notes.Trim());
        var poseFile = await CreateNewDefaultPose(newCharacter.PublicId, request.File, ct);
        await unitOfWork.WithTransactionAsync(async () =>
        {
            newCharacter.SetDefaultPose(poseFile);
            await characterRepo.CreateAsync(newCharacter, ct);
        }, ct);

        await fileManagerCache.RefreshImage(poseFile, ct: ct);
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
