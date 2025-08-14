using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.BankAccountVerifications.Queries.GetBankAccountProfile;

public sealed class GetBankAccountProfileHandler(
    IUserAccessor userAccessor,
    IUserBankAccountRepo userBankAccountRepo,
    IFileStorageService fileStorage) : IQueryHandler<GetBankAccountProfileQuery, GetBankAccountProfileResult>
{
    public async Task<GetBankAccountProfileResult> Handle(GetBankAccountProfileQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var targetBankAccount = await userBankAccountRepo
            .GetByCurencyCodeAsync(userId, CurrencyUnit.Of(request.Code), ct);

        var result = targetBankAccount.Adapt<GetBankAccountProfileResult>() with
        {
            ImageUrl = await GetImageUrl(targetBankAccount.Image),
        };
        return result;
    }

    private async Task<string> GetImageUrl(MediaFile? file)
    {
        if (file is null) return string.Empty;

        var url = await fileStorage.GenerateDownloadUrlAsync(
            bucketName: file.BucketName,
            objectName: file.ObjectName,
            expiry: TimeSpan.FromHours(1));
        return url;
    }
}
