using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.Caching;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnVerifyCreated;
using game_x.application.Features.Accounts.User.Dtos;
using game_x.application.Features.BankAccountVerifications.Dtos;

namespace game_x.application.Features.BankAccountVerifications.Commands._3_ResubmitBankAccount;

public sealed class ResubmitBankAccountHandler(
    IUnitOfWork unitOfWork,
    IUserBankAccountRepo bankAccountRepo,
    IUserAccessor userAccessor,
    IMediaFileRepo mediaFileRepo,
    IApplicationEventDispatcher eventDispatcher,
    IFileStorageService fileStorage,
    IFileManagerCacheService fileManagerCache) : ICommandHandler<ResubmitBankAccountCommand>
{
    public async Task<Unit> Handle(ResubmitBankAccountCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        UserBankAccount? updateBankAccount = null;

        await unitOfWork.WithTransactionAsync(async () =>
        {
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
        }, ct);

        if (updateBankAccount is null) return Unit.Value;

        // Refresh image cache
        if (updateBankAccount.Image != null)
            await fileManagerCache.RefreshImage(updateBankAccount.Image, ct: ct);

        // Publish event after submission
        var bankAccountAfterUpdated = await bankAccountRepo.GetByIdAsync(updateBankAccount.PublicId, ct);
        var userBankAccountItemDto = bankAccountAfterUpdated?.Adapt<BankAccountListItemDto>();
        await eventDispatcher.Publish(new OnVerifyCreatedEvent(userId, VerificationStatusType.BankAccount, null, userBankAccountItemDto), ct);

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
