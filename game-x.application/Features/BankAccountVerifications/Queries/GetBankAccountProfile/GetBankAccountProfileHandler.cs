using game_x.application.Contract.Infrastructure.FileStorage;
using game_x.application.Contract.Infrastructure.Security;
using game_x.application.Contract.Persistence.Repo;

namespace game_x.application.Features.BankAccountVerifications.Queries.GetBankAccountProfile;

public sealed class GetBankAccountProfileHandler(
    IUserAccessor userAccessor,
    IUserRepo userRepo,
    IFileStorageService fileStorage) : IQueryHandler<GetBankAccountProfileQuery, GetBankAccountProfileResult>
{
    public async Task<GetBankAccountProfileResult> Handle(GetBankAccountProfileQuery request, CancellationToken ct = default)
    {
        var userId = userAccessor.GetUserId();
        var targetProfile = await userRepo.GetKycProfileAsync(userId, ct);

        var result = targetProfile.Adapt<GetBankAccountProfileResult>() with
        {
            ImageUrl = await GetImageUrl(targetProfile.FrontImage),
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
