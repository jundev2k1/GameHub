using game_x.application.Common.Files;
using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;
using game_x.application.Events.OnVerifyCreated;
using game_x.application.Features.Accounts.User.Dtos;

namespace game_x.application.Features.BankAccountVerifications.Commands._1_SubmitBankAccount;

public sealed class SubmitBankAccountHandler(
    IUserAccessor userAccessor,
    IUnitOfWork unitOfWork,
    IUserRepo userRepo,
    IFiatCurrencyRepo currencyRepo,
    IApplicationEventDispatcher eventDispatcher,
    IFileStorageService fileStorage) : ICommandHandler<SubmitBankAccountCommand>
{
    public async Task<Unit> Handle(SubmitBankAccountCommand request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var targetCurrency = await currencyRepo
            .GetByCodeAsync(CurrencyUnit.Of(request.CurrencyCode), ct);

        var imageObjectName = await UploadFiles(request.Image, userId, ct);
        var user = await userRepo.GetUserByIdAsync(userId, ct);

        await unitOfWork.WithTransactionAsync(async () =>
        {
            await userRepo.UpdateAsync(userId, targetUser =>
            {
                var isExists = targetUser.UserBankAccounts
                    .Any(uba => uba.UserId == userId && uba.FiatCurrency.Code.Equals(targetCurrency.Code));
                if (isExists)
                    throw new BadRequestException(MessageCode.User.BankAccountAlreadyExists);

                var userBankAccount = UserBankAccount.Create(
                    userId: userId,
                    bankName: request.BankName,
                    bankCode: request.BankCode,
                    accountName: request.AccountName,
                    accountNumber: request.AccountNumber,
                    currencyId: targetCurrency.Id);
                userBankAccount.UploadImage(CreateMediaFile(request.Image, imageObjectName));
                userBankAccount.Submit();

                targetUser.AddUserBankAccount(userBankAccount);
            }, ct);

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
        var objectName = ObjectName.BankAccountProfile(userId, newFileName);
        await fileStorage.UploadFileAsync(file.Content, BucketName.User, objectName, MimeType.Of(file.ContentType), ct);
        return objectName;
    }
}
