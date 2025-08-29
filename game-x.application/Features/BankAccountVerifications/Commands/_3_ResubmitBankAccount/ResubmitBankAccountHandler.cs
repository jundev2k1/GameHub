using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnVerifyCreated;
using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Features.BankAccountVerifications.Commands._3_ResubmitBankAccount;

public sealed class ResubmitBankAccountHandler(
    IUnitOfWork unitOfWork,
    IUserBankAccountRepo bankAccountRepo,
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IMediaFileRepo mediaFileRepo,
    IApplicationEventDispatcher eventDispatcher,
    IFileStorageService fileStorage) : ICommandHandler<ResubmitBankAccountCommand>
{
    public async Task<Unit> Handle(ResubmitBankAccountCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var user = await userRepo.GetUserByIdAsync(userId, ct);

        await unitOfWork.WithTransactionAsync(async () =>
        {
            UserBankAccount? updateBankAccount = null;
            await bankAccountRepo.UpdateAsync(userId, CurrencyUnit.Of(request.CurrencyCode), targetBankAccount =>
            {
                if (targetBankAccount.Status == UserBankAccountStatus.NotSubmitted)
                    throw new BadRequestException(MessageCode.User.BankAccountStatusInvalid);

                targetBankAccount.ReSubmit(
                    request.BankName,
                    request.BankCode,
                    request.AccountName,
                    request.AccountNumber);
                updateBankAccount = targetBankAccount;
            }, ct);

            if (request.Image != null)
                await ReupImage(updateBankAccount, request.Image, ct);

            var verificationDto = new VerificationCreatedDto
            {
                Type = Enum.GetName(typeof(VerificationStatusType), VerificationStatusType.BankAccount) ?? string.Empty,
                Email = user.Email!,
                NickName = user.Nickname,
            };

            await eventDispatcher.Publish(new OnVerifyCreatedEvent(userId, verificationDto), ct);
        }, ct);

        return Unit.Value;
    }

    private async Task ReupImage(
        UserBankAccount? targetBankAccount,
        FileUpload file,
        CancellationToken ct)
    {
        if (targetBankAccount == null) return;

        var oldFile = targetBankAccount.Image;
        if (oldFile == null) return;

        var newFile = MediaFile.Create(
            bucketName: oldFile.BucketName,
            objectName: oldFile.ObjectName,
            fileName: file.FileName,
            mimeType: MimeType.Of(file.ContentType),
            sizeBytes: Convert.ToInt32(file.Length));

        // Update new image information
        targetBankAccount.UploadImage(newFile);

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
