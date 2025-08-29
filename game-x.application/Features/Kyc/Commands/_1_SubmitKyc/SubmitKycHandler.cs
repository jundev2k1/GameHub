using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnVerifyCreated;
using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Features.Kyc.Commands._1_SubmitKyc;

public sealed class SubmitKycHandler(
    IUserAccessor userAccessor,
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IApplicationEventDispatcher eventDispatcher,
    IFileStorageService fileStorage) : ICommandHandler<SubmitKycCommand>
{
    public async Task<Unit> Handle(SubmitKycCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();

        var frontObjectName = await UploadFiles(request.FrontImage, userId, ct);
        var backObjectName = await UploadFiles(request.BackImage, userId, ct);
        var user = await userRepo.GetUserByIdAsync(userId, ct);

        await unitOfWork.WithTransactionAsync(async () =>
        {
            await userRepo.UpdateAsync(userId, targetUser =>
            {
                if ((targetUser.UserKyc != null) && (targetUser.UserKyc.Status != KycStatus.NotSubmitted))
                    throw new BadRequestException(MessageCode.User.KycInvalidStatus);

                var userKyc = UserKyc.Create(
                    userId: userId,
                    fullName: request.FullName.Trim(),
                    dateOfBirth: request.DateOfBirth,
                    address: request.Address,
                    idNumber: request.IdNumber,
                    type: request.Type);
                userKyc.UploadFrontImage(CreateMediaFile(request.FrontImage, frontObjectName));
                userKyc.UploadBackImage(CreateMediaFile(request.BackImage, backObjectName));
                userKyc.Submit();

                targetUser.AddUserKyc(userKyc);
            }, ct);

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

    private static MediaFile CreateMediaFile(FileUpload file, ObjectName objectName)
    {
        return MediaFile.Create(
            bucketName: BucketName.User,
            objectName: objectName,
            fileName: file.FileName,
            mimeType: MimeType.Of(file.ContentType),
            sizeBytes: Convert.ToInt32(file.Length));
    }

    private async Task<ObjectName> UploadFiles(FileUpload file, string userId, CancellationToken ct)
    {
        var newFileName = Guid.NewGuid().ToString() + file.Extension;
        var objectName = ObjectName.KycProfile(userId, newFileName);
        await fileStorage.UploadFileAsync(file.Content, BucketName.User, objectName, MimeType.Of(file.ContentType), ct);
        return objectName;
    }
}
