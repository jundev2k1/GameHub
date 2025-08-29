using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnVerifyCreated;
using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Features.Kyc.Commands._3_ResubmitKyc;

public sealed class ResubmitKycHandler(
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IUserAccessor userAccessor,
    IMediaFileRepo mediaFileRepo,
    IApplicationEventDispatcher eventDispatcher,
    IFileStorageService fileStorage) : ICommandHandler<ResubmitKycCommand>
{
    public async Task<Unit> Handle(ResubmitKycCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var user = await userRepo.GetUserByIdAsync(userId, ct);

        await unitOfWork.WithTransactionAsync(async () =>
        {
            UserKyc? updateKyc = null;
            await userRepo.UpdateKycAsync(userId, targetKyc =>
            {
                targetKyc.Resubmit(
                    request.FullName,
                    request.DateOfBirth,
                    request.Address,
                    request.IdNumber,
                    request.Type);
                updateKyc = targetKyc;
            }, ct);

            if (request.FrontImage != null)
                await ReupImage(updateKyc, request.FrontImage, true, ct);

            if (request.BackImage != null)
                await ReupImage(updateKyc, request.BackImage, false, ct);

            var verificationDto = new VerificationCreatedDto
            {
                Type = Enum.GetName(typeof(VerificationStatusType), VerificationStatusType.Kyc) ?? string.Empty,
                Email = user.Email!,
                NickName = user.Nickname,
            };

            await eventDispatcher.Publish(new OnVerifyCreatedEvent(userId, verificationDto), ct);
        }, ct);

        return Unit.Value;
    }

    private async Task ReupImage(
        UserKyc? targetKyc,
        FileUpload file,
        bool isFront,
        CancellationToken ct)
    {
        if (targetKyc == null) return;

        var oldFile = isFront ? targetKyc.FrontImage : targetKyc.BackImage;
        if (oldFile == null) return;

        var newFile = MediaFile.Create(
            bucketName: oldFile.BucketName,
            objectName: oldFile.ObjectName,
            fileName: file.FileName,
            mimeType: MimeType.Of(file.ContentType),
            sizeBytes: Convert.ToInt32(file.Length));

        // Update new image information
        if (isFront)
            targetKyc.UploadFrontImage(newFile);
        else
            targetKyc.UploadBackImage(newFile);

        // Upload new image, overwrite old file
        await fileStorage.UploadFileAsync(
            fileStream: file.Content,
            bucketName: newFile.BucketName,
            objectName: newFile.ObjectName,
            mimeType: MimeType.Of(file.ContentType),
            ct);

        // Delete old image
        await mediaFileRepo.RemoveAsync(oldFile.Id, ct);
    }
}
