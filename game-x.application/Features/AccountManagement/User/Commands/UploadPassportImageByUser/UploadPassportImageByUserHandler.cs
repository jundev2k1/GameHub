using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.AccountManagement.User.Commands.UploadPassportImageByUser;

public sealed class UploadPassportImageByUserHandler(
    IFileStorageService fileStorageService,
    IMediaFileRepo mediaFileRepo,
    IUserPassportRepo userPassportRepo,
    IUnitOfWork unitOfWork) : ICommandHandler<UploadPassportImageByUserCommand, UploadPassportImageResult>
{
    public async Task<UploadPassportImageResult> Handle(UploadPassportImageByUserCommand request, CancellationToken ct)
    {
        var isExist = await userPassportRepo.IsExistsByPassportNumberAsync(request.PassportNumber, ct);
        if (!isExist)
            throw new NotFoundException(nameof(UserPassport), nameof(UserPassport.PassportNumber));

        var bucketName = BucketName.Of("user");
        var file = request.File;
        var cleanFileName = Path.GetFileName(file.FileName);
        var objectName = ObjectName.Of($"user-passport/{cleanFileName}");
        var mimeType = MimeType.Of(file.ContentType);
        await fileStorageService.UploadFileAsync(file.Content, bucketName, objectName, mimeType, ct);

        var targetPassport = await userPassportRepo.GetByPassportNumberAsync(request.PassportNumber, ct);
        if (targetPassport.PassportImageId.HasValue)
        {
            var isExistFile = targetPassport.PassportImageId.HasValue;
            if (isExistFile)
                await mediaFileRepo.RemoveAsync(targetPassport.PassportImageId!.Value, ct);
        }

        await userPassportRepo.UpdatePassportAsync(request.PassportNumber, passport =>
        {
            var media = MediaFile.Create(bucketName, objectName, cleanFileName, mimeType, (int)file.Length, "{}");
            passport.PassportImage = media;
        }, ct);
        await unitOfWork.SaveChangesAsync(ct);

        var downloadUrl = await fileStorageService.GenerateDownloadUrlAsync(bucketName, objectName, TimeSpan.FromHours(1), ct);
        return new UploadPassportImageResult("Image updated successfully.", downloadUrl);
    }
}
